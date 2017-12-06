using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AR_UI_Text : MonoBehaviour
{
    public Text UIValue;
    private TextMesh UITextMesh;
    // Use this for initialization
    void Start()
    {
        UITextMesh = this.GetComponent<TextMesh>();
        UITextMesh.text = UIValue.text;
    }

    // Update is called once per frame
    void Update()
    {
        UITextMesh.text = UIValue.text;
    }
}
