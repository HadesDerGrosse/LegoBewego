﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroStone : MonoBehaviour {

    private static HeroStone instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public static HeroStone getInstance()
    {
        return instance;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        GameManager.getInstance().distanceTreveld = Mathf.Max(transform.position.x*1.6f, GameManager.getInstance().distanceTreveld);

        if(transform.position.y < -2)
        {
            GameManager.getInstance().endGame();
        }
		
	}

    void OnDestroy()
    {
       
    }
}
