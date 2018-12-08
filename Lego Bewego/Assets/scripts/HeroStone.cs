using System.Collections;
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

        GameManager.getInstance().distanceTreveld = Mathf.Max(transform.position.x, GameManager.getInstance().distanceTreveld);
		
	}

    void OnDestroy()
    {
        VectorField.instance.removeParticle(GetComponent<Rigidbody>());
        GameManager.getInstance().endGame();
    }
}
