using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyParticle : MonoBehaviour {

	// Use this for initialization
	void Start () {
        VectorField.instance.addParticle(GetComponent<Rigidbody>());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
