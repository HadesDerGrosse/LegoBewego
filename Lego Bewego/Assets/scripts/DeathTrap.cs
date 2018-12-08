using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrap : MonoBehaviour {

    public GameObject explosion;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "stone")
        {
            VectorField.instance.removeParticle(collision.rigidbody);
            Destroy(collision.gameObject);
        }
        GameObject go = Instantiate(explosion);
        go.transform.position = transform.position;
        Destroy(go, 5);
        Destroy(this.gameObject);
    }
}
