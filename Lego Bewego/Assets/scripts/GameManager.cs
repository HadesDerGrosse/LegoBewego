using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private static GameManager instance;
    public float distanceTreveld;
    private bool gameIsRunning = false;

    public Canvas currentScoreCanvas;
    public Canvas gameEndCanvas;
    public Canvas startGameCanvas;
    public Text endTimeTextField;
    public Text currentScoreTextField;

    public float damageMultiply=1;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (gameIsRunning)
        {
            currentScoreTextField.text = distanceTreveld.ToString("0.00") + "m";
        }
		
	}


    public static GameManager getInstance()
    {
        return instance;
    }

    public void startGame()
    {
        gameIsRunning = true;
        currentScoreCanvas.gameObject.SetActive(true);
        startGameCanvas.gameObject.SetActive(false);
    }

    public void endGame()
    {
        gameIsRunning = false;
        gameEndCanvas.gameObject.SetActive(true);
        currentScoreCanvas.gameObject.SetActive(false);
        endTimeTextField.text = distanceTreveld.ToString("0.00") + "m";
    }

    public void startLevel()
    {
        startGameCanvas.gameObject.SetActive(true);
        distanceTreveld = 0;
        Application.LoadLevel(index: 1);
    }

    public void quitGame()
    {
        Application.LoadLevel(index: 0);
    }
}
