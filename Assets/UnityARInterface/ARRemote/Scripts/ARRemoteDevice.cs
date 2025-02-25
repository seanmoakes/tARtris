﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking.PlayerConnection;
using Utils;

// Runs on the remote device. Talks to the Editor.
namespace UnityARInterface
{
    public class ARRemoteDevice : MonoBehaviour
    {
        [SerializeField]
        protected Camera m_ARCamera;

        private bool m_SendVideo;
        private ARInterface.Settings m_CachedSettings;
        private ARInterface.PointCloud m_PointCloud;
        private ARInterface.CameraImage m_CameraImage;
        private bool m_HaveSentCameraParams;

        private ARInterface m_ARInterface;
        private PlayerConnection m_PlayerConnection;
        private int m_EditorId;
        public bool isConnected { get { return m_PlayerConnection.isConnected; } }
        private bool m_ServiceRunning = false;
        private Dictionary<Guid, UnityAction<SerializableSubMessage>> m_MessageHandler =
            new Dictionary<Guid, UnityAction<SerializableSubMessage>>();

        void Register(Guid guid, UnityAction<SerializableSubMessage> handler)
        {
            m_MessageHandler.Add(guid, handler);
        }

        void Start()
        {
            Debug.Log("Connecting to editor...");
            m_EditorId = -1;
            m_PlayerConnection = PlayerConnection.instance;
            m_PlayerConnection.RegisterConnection(EditorConnectedEventHandler);
            m_PlayerConnection.RegisterDisconnection(EditorDisconnectedEventHandler);
            m_PlayerConnection.Register(ARMessageIds.fromEditor, FromEditorMessageHandler);

            // The PlayerConnection object can only register a single callback
            // (this is a bug). To listen to more than one type of message,
            // we register a generic one in the PlayerConnection and then
            // decode its meaning with these "SubMessageIds".
            Register(ARMessageIds.SubMessageIds.startService, StartServiceMessageHandler);
            Register(ARMessageIds.SubMessageIds.stopService, StopServiceMessageHandler);
            Register(ARMessageIds.SubMessageIds.enableVideo, EnableVideoMessageHandler);
        }

        private void OnDisable()
        {
            if (m_PlayerConnection != null)
                m_PlayerConnection.DisconnectAll();
        }

        void OnGUI()
        {
            string message = "";
            if (isConnected && !m_ServiceRunning)
            {
                message = "Connected. Waiting for Editor.";
            }
            else if (!isConnected)
            {
                message = "Waiting for editor connection...";
            }
            else
            {
                return;
            }

            var rect = new Rect((Screen.width / 2) - 200, (Screen.height / 2), 400, 50);
            GUI.Box(rect, message);
        }

        void StopServiceMessageHandler(SerializableSubMessage message)
        {
            if (m_ARInterface != null)
                StopService();
        }

        void StopService()
        {
            m_ARInterface.StopService();
            ARInterface.planeAdded -= PlaneAddedHandler;
            ARInterface.planeUpdated -= PlaneUpdatedHandler;
            ARInterface.planeRemoved -= PlaneRemovedHandler;
            m_ServiceRunning = false;
            m_ARInterface = null;
            m_HaveSentCameraParams = false;
        }

        void EnableVideoMessageHandler(SerializableSubMessage message)
        {
            var requestedVideoState = message.GetDataAs<SerializableEnableVideo>();
            m_SendVideo = requestedVideoState.enableVideo;
            m_HaveSentCameraParams = false;
        }

        void StartServiceMessageHandler(SerializableSubMessage message)
        {
            var settings = message.GetDataAs<SerializableARSettings>();
            if (settings == null)
                return;

            if (m_ServiceRunning)
            {
                Debug.LogWarning("Received message to start service while service is already running. Restarting.");
                m_ARInterface.StopService();
            }

            StartService(settings);
        }

        void FromEditorMessageHandler(MessageEventArgs args)
        {
            var subMessage = args.data.Deserialize<SerializableSubMessage>();
            if (subMessage != null)
            {
                UnityAction<SerializableSubMessage> handler = null;
                if (m_MessageHandler.TryGetValue(subMessage.subMessageId, out handler))
                {
                    handler.Invoke(subMessage);
                }
            }
        }

        void StartService(SerializableARSettings serializedSettings)
        {
            m_CachedSettings = serializedSettings;
            var arInterface = ARInterface.GetInterface();
            m_ServiceRunning = arInterface.StartService(m_CachedSettings);

            if (!m_ServiceRunning)
                return;

            m_ARInterface = arInterface;
            m_ARInterface.SetupCamera(m_ARCamera);

            ARInterface.planeAdded += PlaneAddedHandler;
            ARInterface.planeUpdated += PlaneUpdatedHandler;
            ARInterface.planeRemoved += PlaneRemovedHandler;
        }

        public void PlaneAddedHandler(BoundedPlane plane)
        {
            SerializableBoundedPlane serializedPlane = plane;
            SendToEditor(ARMessageIds.addPlane, serializedPlane);
        }

        public void PlaneUpdatedHandler(BoundedPlane plane)
        {
            SerializableBoundedPlane serializedPlane = plane;
            SendToEditor(ARMessageIds.updatePlane, serializedPlane);
        }

        public void PlaneRemovedHandler(BoundedPlane plane)
        {
            SerializableBoundedPlane serializedPlane = plane;
            SendToEditor(ARMessageIds.removePlane, serializedPlane);
        }

        void EditorConnectedEventHandler(int playerId)
        {
            m_EditorId = playerId;
        }

        void EditorDisconnectedEventHandler(int playerId)
        {
            if (m_EditorId == playerId)
                m_EditorId = -1;

            DisconnectFromEditor();

            if (m_ARInterface != null)
                StopService();
        }

        public void DisconnectFromEditor()
        {
#if UNITY_2017_1_OR_NEWER
            m_PlayerConnection.DisconnectAll();
#endif
        }

        public void SendToEditor(System.Guid msgId, byte[] data)
        {
            if (m_PlayerConnection.isConnected)
                m_PlayerConnection.Send(msgId, data);
        }

        public void SendToEditor(System.Guid msgId, object serializableObject)
        {
            byte[] arrayToSend = serializableObject.SerializeToByteArray();
            SendToEditor(msgId, arrayToSend);
        }

        void OnEnable()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // See if we are on a camera
            if (m_ARCamera == null)
                m_ARCamera = GetComponent<Camera>();

            // Fallback to main camera
            if (m_ARCamera == null)
                m_ARCamera = Camera.main;
        }

        private void Update()
        {
            if (m_ARInterface == null)
                return;
            
            m_ARInterface.Update();
            m_ARInterface.UpdateCamera(m_ARCamera);

            Pose pose = new Pose();
            if (m_ARInterface.TryGetPose(ref pose))
            {
                m_ARCamera.transform.position = pose.position;
                m_ARCamera.transform.rotation = pose.rotation;
            }

            if (isConnected && m_ServiceRunning)
            {
                var serializedFrame = new SerializableFrame(
                    m_ARCamera.projectionMatrix,
                    m_ARCamera.transform.position,
                    m_ARCamera.transform.rotation,
					m_ARInterface.GetDisplayTransform());

                SendToEditor(ARMessageIds.frame, serializedFrame);

                if (m_CachedSettings.enablePointCloud)
                {
                    if (m_ARInterface.TryGetPointCloud(ref m_PointCloud))
                    {
                        var serializedPointCloud = new SerializablePointCloud(m_PointCloud);
                        SendToEditor(ARMessageIds.pointCloud, serializedPointCloud);
                    }
                }

                if (m_CachedSettings.enableLightEstimation)
                {
                    var serializedLightEstimate = (SerializableLightEstimate)m_ARInterface.GetLightEstimate();
                    SendToEditor(ARMessageIds.lightEstimate, serializedLightEstimate);
                }

                if (m_SendVideo)
                {
                    if (m_ARInterface.TryGetCameraImage(ref m_CameraImage))
                    {
                        if (!m_HaveSentCameraParams)
                        {
                            var screenCaptureParams = new SerializableScreenCaptureParams(
                                m_CameraImage.width,
                                m_CameraImage.height,
                                (int)TextureFormat.YUY2);

                            SendToEditor(ARMessageIds.screenCaptureParams, screenCaptureParams);
                            m_HaveSentCameraParams = true;
                        }

                        SendToEditor(ARMessageIds.screenCaptureY, m_CameraImage.y);
                        SendToEditor(ARMessageIds.screenCaptureUV, m_CameraImage.uv);
                    }
                }
            }
        }
    }
}
