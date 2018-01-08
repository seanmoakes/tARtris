using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTARtrimino : MonoBehaviour
{
    private Tartris tartrisRef;
    Transform currentActiveTARtrimoTransform;
    void Start()
    {
        tartrisRef = FindObjectOfType<Tartris>();
        tag = "currentGhostTARtrimino";
        currentActiveTARtrimoTransform = GameObject.FindGameObjectWithTag("currentActiveTARtrimino").transform;
        //foreach (Transform mino in transform)
        //{
        //    if (mino != transform.GetChild(4))
        //    {
        //        mino.GetComponent<MeshRenderer>().material = tartrisRef.materials[0];
        //    }
        //}
    }
    void LateUpdate()
    {
        if (tartrisRef.updateGhost)
        {
            FollowActiveTARtrimino();
            MoveDown();
            tartrisRef.updateGhost = false;
        }
    }
    void FollowActiveTARtrimino()
    {
        //Transform currentActiveTARtrimoTransform = GameObject.FindGameObjectWithTag("currentActiveTARtrimino").transform;

        transform.position = currentActiveTARtrimoTransform.position;
        transform.rotation = currentActiveTARtrimoTransform.rotation;
    }

    void MoveDown()
    {
        while (CheckIsValidPosition())
        {
            transform.position += new Vector3(0, -1, 0);
        }
        if (!CheckIsValidPosition())
        {
            transform.position += new Vector3(0, 1, 0);
        }
        tartrisRef.updateGhost = false;
    }

    public bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            if (mino != transform.GetChild(4))
            {
                Vector2 pos = tartrisRef.RoundVec2(mino.position);
                if (tartrisRef.CheckIsInsideGrid(pos) == false)
                {
                    return false;
                }
            }
        }
        foreach (Transform mino in transform)
        {
            if (mino != transform.GetChild(4))
            {
                Vector2 pos = tartrisRef.RoundVec2(mino.position);
                if (tartrisRef.GetTransformAtGridPosition(pos) != null)
                {
                    if (tartrisRef.GetTransformAtGridPosition(pos)?.parent?.tag != "currentActiveTARtrimino")
                    {
                        return false;
                    }
                    if (tartrisRef.GetTransformAtGridPosition(pos)?.parent?.tag == "currentActiveTARtrimino")
                    {
                        return true;
                    }
                    else if (tartrisRef.GetTransformAtGridPosition(pos)?.parent != transform)
                    {
                        return false;
                    }
                }
            }

        }
        return true;
    }
}