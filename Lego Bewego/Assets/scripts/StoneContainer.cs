using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneContainer : MonoBehaviour {

    public int startHealth = 5;
    public int currentHealth = 0;
    public float damageMulti = 1.0f;
    public List<Transform> children;
    public List<Vector3> startPositions;

	// Use this for initialization
	void Start () {
        currentHealth = startHealth;
        children = new List<Transform>();

        for(int i = 0; i<transform.childCount; i++){
            children.Add(transform.GetChild(i));
            startPositions.Add(transform.GetChild(i).position);
        }
	}
	
	// Update is called once per frame
	void Update () {

        foreach(Transform t in children)
        {
            if(t.position.x > Camera.main.transform.position.x - WorldManager.getInstance().visibleDistance)
            {
                return;
            }
        }

        reset();


	}

    void OnCollisionEnter(Collision collision)
    {
        if(!children.Contains(collision.transform) && collision.gameObject.tag == "stone")
        {
            int damage = Convert.ToInt32(collision.relativeVelocity.magnitude*GameManager.getInstance().damageMultiply);
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                crush(collision.collider.transform.position, damage);
            }
        }             
    }

    public void reset()
    {
        for(int i = 0; i < children.Count; i++)
        {
            Rigidbody rig = children[i].gameObject.GetComponent<Rigidbody>();
            VectorField.instance.removeParticle(rig);
            children[i].position = startPositions[i];
            children[i].localRotation = Quaternion.identity;
            Destroy(rig);
        }

        WorldManager.getInstance().groupyPool.add(this.gameObject);

    }

    public void crush(Vector3 position, float force)
    {
        foreach(Transform child in children)
        {
            if(child.GetComponent<Rigidbody>() == null)
            {
                Rigidbody rig = child.gameObject.AddComponent<Rigidbody>();
                rig.constraints = RigidbodyConstraints.FreezePositionY;
                rig.drag = 0.25f;
                rig.gameObject.tag = "stone";
                rig.AddExplosionForce(force * 20, position, force*2);
                VectorField.instance.addParticle(rig);
                //child.parent = this.transform.parent;
            }            
        }
    }
}
