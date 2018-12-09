using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour {

    public Sprite[] characters;
    public string[] story;

    public Image storyguy;
    public Text storyText;
    public RectTransform parentTransform;

    public float storyEventDistance = 200;

    private float lastStory = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Animation anim = parentTransform.GetComponent<Animation>();
        if (!anim.isPlaying && lastStory + storyEventDistance < Camera.main.transform.position.x)
        {
            lastStory = Camera.main.transform.position.x;
            storyguy.sprite = characters[Random.Range(0, characters.Length)];
            storyText.text = story[Random.Range(0,story.Length)];
            anim.Play();
        }
	}
    

}
