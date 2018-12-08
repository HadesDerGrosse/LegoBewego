using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public float maxZ;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {        

        if (HeroStone.getInstance() != null)
        {
            Vector3 heroPos = HeroStone.getInstance().transform.position;            

            float x = Mathf.Lerp(heroPos.x, transform.position.x,0.95f);
            float z = Mathf.Clamp(Mathf.Lerp(heroPos.z, transform.position.z, 0.95f),-maxZ,maxZ);
            float y = transform.position.y;


            transform.position = new Vector3(x, y, z);

        }
            
	}
}
