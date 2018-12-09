using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextRandomizer : MonoBehaviour {

    public string[] texts;

	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = texts[Random.Range(0, texts.Length)];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
