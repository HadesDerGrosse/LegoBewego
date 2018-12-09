using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    public Image[] score;

    public Sprite[] numbers;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        string distance = GameManager.getInstance().distanceTreveld.ToString("0000000000");
        char[] numeralArray = distance.ToCharArray();

        for(int i =0; i < numeralArray.Length; i++)
        {
            score[i].sprite = numbers[numeralArray[i]-'0'];  
        }
	}
}
