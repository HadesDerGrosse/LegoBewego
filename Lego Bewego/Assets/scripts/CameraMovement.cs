using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {        

        if (HeroStone.getInstance() != null)
        {
            Vector3 heroPos = HeroStone.getInstance().transform.position;
            this.transform.position = Vector3.Lerp(new Vector3(heroPos.x, transform.position.y, heroPos.z), transform.position, 0.95f);
        }
            
	}
}
