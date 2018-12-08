using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrap : MonoBehaviour {

    public GameObject explosion;
    public float range;
    public float force;
    public Vector3 angleSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(angleSpeed*Time.deltaTime);
	}

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "stone")
        {
            VectorField.instance.removeParticle(collision.rigidbody);
            collision.rigidbody.constraints = RigidbodyConstraints.None;
            Destroy(collision.gameObject,5);

            GameObject go = Instantiate(explosion);
            go.transform.position = transform.position;
            Destroy(go, 5);

            foreach (Collider c in Physics.OverlapSphere(transform.position, range))
            {
                if (c.GetComponent<Rigidbody>() != null)
                {
                    c.GetComponent<Rigidbody>().AddExplosionForce(force, transform.position, range);
                    Debug.Log(c.transform.name);
                }
            }

            WorldManager.minePool.add(this.gameObject);
        }
        
    }
}
