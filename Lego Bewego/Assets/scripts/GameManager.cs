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
    public ScoreHandler currentScoreHandler;


    public float damageMultiply=1;

    public int[] highscore = { 0, 0, 0 };

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
            currentScoreHandler.setImages(Mathf.RoundToInt(distanceTreveld));

            if (HeroStone.getInstance().transform.position.x < Camera.main.transform.position.x - WorldManager.getInstance().visibleDistance)
            {
                endGame();
            }
        }

        
		
	}


    public static GameManager getInstance()
    {
        return instance;
    }

    public void startGame()
    {
        gameIsRunning = true;
        VectorField.instance.clearParticles();
        VectorField.instance.addParticle(HeroStone.getInstance().GetComponent<Rigidbody>());        
        currentScoreCanvas.gameObject.SetActive(true);
        startGameCanvas.gameObject.SetActive(false);
        AudioManager.instance.setIngame();
        CameraMovement.instance.StartAutoMove();
    }

    public void endGame()
    {
        gameIsRunning = false;
        gameEndCanvas.gameObject.SetActive(true);
        currentScoreCanvas.gameObject.SetActive(false);
        HeroStone.getInstance().enabled = false;
        CameraMovement.instance.StopAutoMove();

        AudioManager.instance.setMenue();

        int score = Mathf.RoundToInt(distanceTreveld);
        highscore[0] = PlayerPrefs.GetInt("1", 0);
        highscore[1] = PlayerPrefs.GetInt("2", 0);
        highscore[2] = PlayerPrefs.GetInt("3", 0);

        for(int i=0; i<3; i++)
        {
            if(score > highscore[i])
            {
                highscore[i] = score;
                break;
            }
        }

        PlayerPrefs.SetInt("1", highscore[0]);
        PlayerPrefs.SetInt("2", highscore[1]);
        PlayerPrefs.SetInt("3", highscore[2]);
    }

    public void startLevel(){

        Application.LoadLevel(index: 1);
        VectorField.instance.clearParticles();               
        startGameCanvas.gameObject.SetActive(true);
        gameEndCanvas.gameObject.SetActive(false);
        distanceTreveld = 0;
        
    }

    public void quitGame()
    {
        Application.LoadLevel(index: 0);
    }
}
