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

        //Testing with other scene
//      SceneManager.LoadScene("tetris3d");
    }
}
