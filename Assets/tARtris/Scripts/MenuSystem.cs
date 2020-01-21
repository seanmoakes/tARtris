using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    public void PlayAgain()
    {
        //AR
        SceneManager.LoadScene("tARtrisScene");

        //Test with standard scene
        //SceneManager.LoadScene("tetris3d");
    }

    public void ReturnToMenu()
    {
        Destroy(GameObject.FindGameObjectWithTag("ARRoot"));
        SceneManager.LoadScene("GameMenu");
    }
}
