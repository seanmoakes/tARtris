using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTARtrimino : MonoBehaviour
{
// 
//     void Start()
//     {
//         tag = "currentGhostTARtrimino";
//         foreach (Transform mino in transform)
//         {
//             if (mino != transform.GetChild(4))
//             {
//                 mino.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, .2f);
//             }
//         }
//     }
//     void Update()
//     {
//         FollowActiveTARtrimino();
// 
//     }
//     void FollowActiveTARtrimino()
//     {
//         Transform currentActiveTARtrimoTransform = GameObject.FindGameObjectWithTag("currentActiveTARtrimino").transform;
// 
//         transform.position = currentActiveTARtrimoTransform.position;
//         transform.rotation = currentActiveTARtrimoTransform.rotation;
//         MoveDown();
//     }
// 
//     void MoveDown()
//     {
//         while (CheckIsValidPosition())
//         {
//             transform.position += new Vector3(0, -1, 0);
//         }
//         if (!CheckIsValidPosition())
//         {
//             transform.position += new Vector3(0, 1, 0);
//         }
//     }
// 
//     public bool CheckIsValidPosition()
//     {
//         foreach (Transform child in transform)
//         {
//             if (child != transform.GetChild(4))
//             {
//                 Vector2 v = TetrisGrid.roundVec2(child.position);
//                 if (!TetrisGrid.insideBorder(v))
//                 {
//                     return false;
//                 }
//                 // 
//                 //                 if (TetrisGrid.grid[(int)v.x, (int)v.y] == null)
//                 //                     return true;
//                 //                 
//                 if (TetrisGrid.grid[(int)v.x, (int)v.y].transform.parent.tag == "currentActiveTARtrimino")
//                 {
//                     return true;
//                 }
//                 //                 
//                 //                 {
//                 //                     return false;
//                 //                 }
// 
//             }
//         }
//         return true;
//     }
}