using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public AudioSource inGame;
    public AudioSource menue;
    public AudioSource death;
    public AudioSource click;
    public AudioSource groupy;



    public static AudioManager instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

	// Use this for initialization
	void Start () {
        setMenue();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setIngame()
    {
        menue.enabled = false;
        inGame.enabled = true;
    }

    public void setMenue()
    {
        menue.enabled = true;
        inGame.enabled = false;
    }

    public void playGroupy()
    {
        groupy.Play();
    }

    public void playClick()
    {
        click.Play();
    }
}
