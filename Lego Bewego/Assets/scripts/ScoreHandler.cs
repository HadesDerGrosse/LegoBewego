using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour {

    public Image[] images;

	// Update is called once per frame
	void Update () {
		
	}

    public void setImages(int value)
    {
        Sprite[] sprites = ScoreManager.intToSpriteArray(value);

        for (int i=0; i<10; i++)
        {
            images[i].sprite = sprites[i];
        }
    }
}
