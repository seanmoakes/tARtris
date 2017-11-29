using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Groups
    public GameObject[] tetrominos;
    public Color[] tetrominoColors;
    private MaterialPropertyBlock props;
    // private bool gameOver = false;
    private GameObject ghost;
    public void spawnNext()
    {
        if (!TARtrisManager.IsGameOver())
        {
            // Select group to spawn using a random number
            int i = Random.Range(0, tetrominos.Length);
            GameObject tetrominoGO = Instantiate(tetrominos[i], transform.position, Quaternion.identity);
            tetrominoGO.tag = "currentActiveTARtrimino";
            props.SetColor("_InstanceColor", tetrominoColors[i]);
            var renderers = tetrominoGO.GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer renderer in renderers)
            {
                renderer.SetPropertyBlock(props);
            }

            if (GameObject.FindGameObjectWithTag("currentGhostTARtrimino") != null)
                Destroy(GameObject.FindGameObjectWithTag("currentGhostTARtrimino"));
            ghost = (GameObject)Instantiate(tetrominoGO, tetrominoGO.transform.position, Quaternion.identity);
            Destroy(ghost.GetComponent<Tetromino>());
            ghost.AddComponent<GhostTARtrimino>();
            //gameOver = TARtrisManager.IsGameOver();
        }
    }

    void Start()
    {
        props = new MaterialPropertyBlock();
        spawnNext();
    }
}
