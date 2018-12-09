using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour {

    public Vector3 dimensions;
    // Use this for initialization

    public List<Transform> minePositions;
    public List<Transform> decoPosition;

    public float minePossibility;

    void Awake()
    {
        Transform logicHRC = transform.GetChild(0);
        for (int i = 0; i < logicHRC.childCount; i++)
        {            
            if (logicHRC.GetChild(i).name.Contains("_mine_"))
                minePositions.Add(logicHRC.GetChild(i));

            if (logicHRC.GetChild(i).name.Contains("_deco_") || logicHRC.GetChild(i).name.Contains("_socket_"))
                decoPosition.Add(logicHRC.GetChild(i));

            if (logicHRC.GetChild(i).name.Contains("UBX"))
            {
                MeshCollider mc = logicHRC.GetChild(i).gameObject.AddComponent<MeshCollider>();
                MeshRenderer mr = logicHRC.GetChild(i).gameObject.GetComponent<MeshRenderer>();
                MeshFilter mf = logicHRC.GetChild(i).gameObject.GetComponent<MeshFilter>();
                mc.sharedMesh = mf.sharedMesh;
                mr.enabled = false;

            }
        }

        calculateDimensions();
    }
    
	void Start () {
        

        placeMines();
	}

    public void placeMines()
    {
        foreach (Transform t in minePositions)
        {
            if (UnityEngine.Random.Range(0.0f, 1.0f) < minePossibility)
            {
                GameObject go = WorldManager.minePool.get();
                go.transform.position = t.position;

            }

        }
    }

   private void calculateDimensions()
    {
        Vector3 min = Vector3.zero;
        Vector3 max = Vector3.zero;
        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            min = new Vector3(Mathf.Min(c.bounds.min.x, min.x), Mathf.Min(c.bounds.min.y, min.y), Mathf.Min(c.bounds.min.z, min.z));
            max = new Vector3(Mathf.Max(c.bounds.max.x, max.x), Mathf.Max(c.bounds.max.y, max.y), Mathf.Max(c.bounds.max.z, max.z));
        }

        dimensions = max - min;
    }
}
