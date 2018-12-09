using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dirtyCall : MonoBehaviour {

	void OnPostRender()
    {
        VectorField.instance.DrawLines();
    }
}
