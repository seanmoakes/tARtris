using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HiScore : MonoBehaviour {
    public Text ScoreValue;
    private TextMesh scoreMesh;
	// Use this for initialization
	void Start () {
        scoreMesh = this.GetComponent<TextMesh>();
        scoreMesh.text = ScoreValue.text;
	}
	
	// Update is called once per frame
	void Update () {
        scoreMesh.text = ScoreValue.text;
	}
}
