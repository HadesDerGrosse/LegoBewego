using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public AudioClip inGame;
    public AudioClip menue;

    private AudioSource source;

    public static AudioManager instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        source = GetComponent<AudioSource>();
    }

	// Use this for initialization
	void Start () {
        source.clip = menue;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setIngame()
    {
        source.clip = inGame;
        source.Play();
    }

    public void setMenue()
    {
        source.clip = menue;
        source.Play();
    }
}
