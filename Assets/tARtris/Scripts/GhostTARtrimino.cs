using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTARtrimino : MonoBehaviour
{

    void Start()
    {
        tag = "currentGhostTARtrimino";
        foreach (Transform mino in transform)
        {
            if (mino != transform.GetChild(4))
            {
                mino.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, .2f);
            }
        }
    }
    void Update()
    {
        FollowActiveTARtrimino();
        MoveDown();
    }
    void FollowActiveTARtrimino()
    {
        Transform currentActiveTARtrimoTransform = GameObject.FindGameObjectWithTag("currentActiveTARtrimino").transform;

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
    }

    public bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            if (mino != transform.GetChild(4))
            {
                Vector2 pos = FindObjectOfType<Tartris>().RoundVec2(mino.position);
                if (FindObjectOfType<Tartris>().CheckIsInsideGrid(pos) == false)
                {
                    return false;
                }
            }
        }
        foreach (Transform mino in transform)
        {
            if (mino != transform.GetChild(4))
            {
                Vector2 pos = FindObjectOfType<Tartris>().RoundVec2(mino.position);
                if (FindObjectOfType<Tartris>().GetTransformAtGridPosition(pos) != null)
                    if(FindObjectOfType<Tartris>().GetTransformAtGridPosition(pos)?.parent?.tag == "currentActiveTARtrimino")
                    {
                        return true;
                    }
                    else if (FindObjectOfType<Tartris>().GetTransformAtGridPosition(pos)?.parent != transform)
                    {
                        return false;
                    }
            }
        }
        return true;
    }
}