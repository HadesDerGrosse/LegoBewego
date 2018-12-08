using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneContainer : MonoBehaviour {

    public int startHealth = 5;
    public int currentHealth = 0;
    public float damageMulti = 1.0f;
    public List<Transform> children;

	// Use this for initialization
	void Start () {
        currentHealth = startHealth;
        children = new List<Transform>();

        for(int i = 0; i<transform.childCount; i++){
            children.Add(transform.GetChild(i));
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        if(!children.Contains(collision.transform) && collision.gameObject.tag == "stone")
        {
            int damage = Convert.ToInt32(collision.relativeVelocity.magnitude*GameManager.getInstance().damageMultiply);
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                crush();
            }
        }             
    }

    public void crush()
    {
        foreach(Transform child in children)
        {
            if(child.GetComponent<Rigidbody>() == null)
            {
                Rigidbody rig = child.gameObject.AddComponent<Rigidbody>();
                rig.constraints = RigidbodyConstraints.FreezePositionY;
                child.parent = this.transform.parent;
            }            
        }

        Destroy(this.gameObject);
    }
}
