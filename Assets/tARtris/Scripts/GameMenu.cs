using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public Text levelText;
    public Text hiscoreText;

    // Use this for initialization
    void Start()
    {
        levelText.text = "1";
        hiscoreText.text = PlayerPrefs.GetInt("HighScore").ToString();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("tARtrisSetupScene");
    }

    public void ChangedValue(float value)
    {
        Tartris.startingLevel = (int)value;
        levelText.text = value.ToString();
    }
}
