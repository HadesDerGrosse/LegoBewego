using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public Sprite[] numbers;

    private static Sprite[] numbersSprites;

    public static ScoreManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            numbersSprites = numbers;
        }
    }

    public static Sprite[] intToSpriteArray(int value)
    {
        string distance = GameManager.getInstance().distanceTreveld.ToString("0000000000");
        char[] numeralArray = distance.ToCharArray();
        Sprite[] sprites = new Sprite[10];

        for (int i = 0; i < numeralArray.Length; i++)
        {
            sprites[i] = numbersSprites[numeralArray[i] - '0'];
        }

        return sprites;
    }
}


