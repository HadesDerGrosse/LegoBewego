using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public float maxZ;
    public float deltaXSpeed = 0.1f;
    private float currentXSpeed = 0;

    private bool isAutoMoving =false;

    public static CameraMovement instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {        

        if (HeroStone.getInstance() != null)
        {
            Vector3 heroPos = HeroStone.getInstance().transform.position;

            float xSpeed = HeroStone.getInstance().GetComponent<Rigidbody>().velocity.x*0.2f;

            float x = Mathf.Max(Mathf.Lerp(heroPos.x+xSpeed, transform.position.x,0.95f), transform.position.x+ currentXSpeed);
            float z = Mathf.Clamp(Mathf.Lerp(heroPos.z, transform.position.z, 0.95f),-maxZ,maxZ);
            float y = transform.position.y;


            transform.position = new Vector3(x, y, z);

        }

        if (isAutoMoving)
        {
            currentXSpeed += deltaXSpeed * Time.fixedDeltaTime;
        }        

    }

    public void StartAutoMove()
    {
        isAutoMoving = true;
    }

    public void StopAutoMove()
    {
        isAutoMoving = false;
        currentXSpeed = 0;
    }
}
