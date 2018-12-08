using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneContainer : MonoBehaviour {

    public int startHealth = 5;
    public int currentHealth = 0;
    public float damageMulti = 1.0f;
    public Transform[] children;

	// Use this for initialization
	void Start () {
        currentHealth = startHealth;
        children = new Transform[transform.childCount];

        for(int i = 0; i<transform.childCount; i++){
            children[i] = transform.GetChild(i);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        
        int damage = Convert.ToInt32(collision.relativeVelocity.magnitude);
        Debug.Log(collision.relativeVelocity.magnitude);
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            crush();
        }       
    }

    public void crush()
    {
        foreach(Transform child in children)
        {
            child.parent = this.transform.parent;
        }
    }
}
