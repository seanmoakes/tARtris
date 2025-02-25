﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using Utils;

#if UNITY_EDITOR
using UnityEditor.Networking.PlayerConnection;
using UnityEngine.XR.iOS;

// Runs on the Editor. Talks to the remote Player.
namespace UnityARInterface
{
    public class ARRemoteEditorInterface : ARInterface
    {
        private bool m_SendVideo;
        public bool sendVideo
        {
            get { return m_SendVideo; }
            set
            {
                m_SendVideo = value;
                if (editorConnection != null)
                {
                    SendToPlayer(
                        ARMessageIds.SubMessageIds.enableVideo,
                        new SerializableEnableVideo(sendVideo));
                }
            }
        }

        public EditorConnection editorConnection = null;
        private int m_CurrentPlayerId = -1;
        private SerializableFrame m_Frame = null;
        private Settings m_CachedSettings;
        private Camera m_CachedCamera;
        private ARRemoteVideo m_ARVideo;
        private LightEstimate m_LightEstimate;
        public bool connected { get { return m_CurrentPlayerId != -1; } }
        public int playerId { get { return m_CurrentPlayerId; } }

        private bool m_ServiceRunning = false;
        public bool serviceRunning { get { return m_ServiceRunning; } }

        Texture2D m_RemoteScreenYTexture;
        Texture2D m_RemoteScreenUVTexture;

        List<Vector3> m_PointCloud;
		private Matrix4x4 m_DisplayTransform;

        public void ScreenCaptureParamsMessageHandler(MessageEventArgs message)
        {
            var screenCaptureParams = message.data.Deserialize<SerializableScreenCaptureParams>();

            if (m_RemoteScreenYTexture == null ||
                m_RemoteScreenYTexture.width != screenCaptureParams.width ||
                m_RemoteScreenYTexture.height != screenCaptureParams.height)
            {
                if (m_RemoteScreenYTexture != null)
                    GameObject.Destroy(m_RemoteScreenYTexture);

                m_RemoteScreenYTexture = new Texture2D(
                    screenCaptureParams.width,
                    screenCaptureParams.height,
                    TextureFormat.R8, false, true);
            }

            if (m_RemoteScreenUVTexture == null ||
                m_RemoteScreenUVTexture.width != screenCaptureParams.width ||
                m_RemoteScreenUVTexture.height != screenCaptureParams.height)
            {
                if (m_RemoteScreenUVTexture != null)
                    GameObject.Destroy(m_RemoteScreenUVTexture);

                m_RemoteScreenUVTexture = new Texture2D(
                    screenCaptureParams.width / 2,
                    screenCaptureParams.height / 2,
                    TextureFormat.RG16, false, true);
            }

            m_ARVideo = m_CachedCamera.GetComponent<ARRemoteVideo>();
            if (m_ARVideo == null)
            {
                m_ARVideo = m_CachedCamera.gameObject.AddComponent<ARRemoteVideo>();
                m_ARVideo.clearMaterial = Resources.Load("YUVMaterial", typeof(Material)) as Material;
                m_CachedCamera.clearFlags = CameraClearFlags.Depth;
            }

            m_ARVideo.videoTextureY = m_RemoteScreenYTexture;
            m_ARVideo.videoTextureCbCr = m_RemoteScreenUVTexture;
        }

        public void ScreenCaptureYMessageHandler(MessageEventArgs message)
        {
            if (m_RemoteScreenYTexture == null)
                return;

            m_RemoteScreenYTexture.LoadRawTextureData(message.data);
            m_RemoteScreenYTexture.Apply();
        }

        public void ScreenCaptureUVMessageHandler(MessageEventArgs message)
        {
            if (m_RemoteScreenUVTexture == null)
                return;

            m_RemoteScreenUVTexture.LoadRawTextureData(message.data);
            m_RemoteScreenUVTexture.Apply();
        }

        public void PlaneAddedMessageHandler(MessageEventArgs message)
        {
            OnPlaneAdded(message.data.Deserialize<SerializableBoundedPlane>());
        }

        public void PlaneUpdatedMessageHandler(MessageEventArgs message)
        {
            OnPlaneUpdated(message.data.Deserialize<SerializableBoundedPlane>());
        }

        public void PlaneRemovedMessageHandler(MessageEventArgs message)
        {
            OnPlaneRemoved(message.data.Deserialize<SerializableBoundedPlane>());
        }

        public void PointCloudMessageHandler(MessageEventArgs message)
        {
            // Copy to internal buffer
            var pointCloud = message.data.Deserialize<SerializablePointCloud>();
            if (m_PointCloud == null)
                m_PointCloud = new List<Vector3>();

            m_PointCloud.Clear();
            m_PointCloud.AddRange(pointCloud.asEnumerable);
        }

        public void LightEstimateMessageHandler(MessageEventArgs message)
        {
            m_LightEstimate = message.data.Deserialize<SerializableLightEstimate>();
        }

        public void FrameMessageHandler(MessageEventArgs message)
        {
            m_Frame = message.data.Deserialize<SerializableFrame>();
        }

        public void PlayerConnectedMessageHandler(EditorConnection editorConnection, int playerId)
        {
            this.editorConnection = editorConnection;
            m_CurrentPlayerId = playerId;
        }

        public void PlayerDisconnectedMessageHandler(int playerId)
        {
            if (m_CurrentPlayerId == playerId)
            {
                m_CurrentPlayerId = -1;
                m_Frame = null;
                editorConnection = null;
            }
        }

        void SendToPlayer(System.Guid msgId, object serializableObject)
        {
            var message = new SerializableSubMessage(msgId, serializableObject);
            var bytesToSend = message.SerializeToByteArray();
            editorConnection.Send(ARMessageIds.fromEditor, bytesToSend);
        }

        public void StartRemoteService(Settings settings)
        {
            sendVideo = m_SendVideo;
            var serializedSettings = (SerializableARSettings)settings;
            SendToPlayer(ARMessageIds.SubMessageIds.startService, serializedSettings);
            m_ServiceRunning = true;
        }

        public void StopRemoteService()
        {
            SendToPlayer(ARMessageIds.SubMessageIds.stopService, null);
            m_ServiceRunning = false;
        }

        //
        // From the ARInterface
        //
        public override bool StartService(Settings settings)
        {
            return true;
        }

        public override void StopService()
        {

        }

        public override void SetupCamera(Camera camera)
        {
            m_CachedCamera = camera;
        }

        public override bool TryGetUnscaledPose(ref Pose pose)
        {
            if (m_Frame != null)
            {
                pose.position = m_Frame.cameraPosition;
                pose.rotation = m_Frame.cameraRotation;
                return true;
            }

            return false;
        }

        public override bool TryGetCameraImage(ref CameraImage cameraImage)
        {
            return false;
        }

        public override bool TryGetPointCloud(ref PointCloud pointCloud)
        {
            if (m_PointCloud == null)
                return false;

            if (pointCloud.points == null)
                pointCloud.points = new List<Vector3>();

            pointCloud.points.Clear();
            pointCloud.points.AddRange(m_PointCloud);
            return true;
        }

        public override LightEstimate GetLightEstimate()
        {
            return m_LightEstimate;
        }

		public override Matrix4x4 GetDisplayTransform()
		{
			if (m_Frame != null) {
				return m_Frame.displayTransform;
			} else {
				return Matrix4x4.identity;
			}
		}

        public override void Update()
        {

        }

        public override void UpdateCamera(Camera camera)
        {
            if (m_Frame != null)
            {
                camera.projectionMatrix = m_Frame.projectionMatrix;
                if (m_ARVideo)
                {
					m_ARVideo.UpdateDisplayTransform(GetDisplayTransform());
                }
            }
        }

    }
}
#endif
