using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour {

    public Image[] characters;
    public string[] story;

    public Image storyguy;
    public Text storyText;
    public RectTransform parentTransform;

    private bool moving = false;
    private bool showing = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("s")) toggleStory();
	}

    public void toggleStory()
    {
        parentTransform.GetComponent<Animation>().Play();
    }

}
