using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartStoneHelper : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().velocity=new Vector3(10, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
