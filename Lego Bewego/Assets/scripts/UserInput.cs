using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInput : MonoBehaviour {

    public Vector2 start;
    public Vector2 end;
    public Image arrow;

    // Use this for initialization
    void Start () {
        arrow.transform.localScale = (Vector3.zero);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            start = Input.mousePosition;
            arrow.rectTransform.anchoredPosition = new Vector3(start.x, start.y, 0);
            arrow.transform.localScale = Vector3.one;

            arrow.rectTransform.sizeDelta = new Vector2((start - (Vector2)Input.mousePosition).magnitude,100);
            arrow.rectTransform.localRotation.SetFromToRotation(start, Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            arrow.rectTransform.sizeDelta = new Vector2((start - (Vector2)Input.mousePosition).magnitude, 100);
            Quaternion aim = Quaternion.FromToRotation(Vector3.up, (Input.mousePosition - new Vector3(start.x, start.y, 0)));
            arrow.rectTransform.localRotation = Quaternion.AngleAxis(90, Vector3.forward) * aim;
        }
        if (Input.GetMouseButtonUp(0))
        {
            end = Input.mousePosition;
            VectorField.instance.addForce(start, end);
            arrow.transform.localScale = (Vector3.zero);
        }
	}

}
