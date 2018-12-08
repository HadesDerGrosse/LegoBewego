using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {


    public List<GameObject> borderTiles;
    public List<GameObject> currentTiles;
    public List<GameObject> pool;

    public int levelHeight = 50;
    private int lastTilePos = 0;
    public int tileHight = 10;
    public int visibleDistance = 2;

    private float currentPos;
    

	// Use this for initialization
	void Start () {
        lastTilePos = -visibleDistance;

        while (lastTilePos < visibleDistance)
        {
            addTileToWorld();
        }
		
	}
	
	// Update is called once per frame
	void Update () {
        currentPos = Camera.main.transform.position.x;

        if (currentPos - currentTiles[0].transform.position.x > visibleDistance)
        {
            addTileToWorld();
            removeTileFromWorld();            
        }

    }

    private void removeTileFromWorld()
    {
        addTileToPool(currentTiles[0]);
        addTileToPool(currentTiles[1]);
        currentTiles.RemoveAt(0);
        currentTiles.RemoveAt(0);
    }

    private void addTileToWorld()
    {
        GameObject tileUp = getTileFromPool();
        GameObject tileDown = getTileFromPool();

        currentTiles.Add(tileUp);
        currentTiles.Add(tileDown);

        tileDown.transform.position = new Vector3(lastTilePos, 0.0f, -levelHeight / 2.0f);
        tileUp.transform.position = new Vector3(lastTilePos, 0.0f, levelHeight / 2.0f);
        tileUp.transform.rotation = Quaternion.Euler(180, 0, 0);
        tileDown.transform.rotation = Quaternion.Euler(0, 0, 0);

        lastTilePos += tileHight;
    }

    public GameObject getTileFromPool()
    {
        GameObject go = null;

        if (pool.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, pool.Count);
            go = pool[index];            
            pool.RemoveAt(index);            
        } 
        
        else
        {
            go = Instantiate(borderTiles[UnityEngine.Random.Range(0, borderTiles.Count)]);
            go.transform.parent = this.transform;
        }

        go.SetActive(true);
        return go;
    }

    public void addTileToPool(GameObject go)
    {
        pool.Add(go);
        go.SetActive(false);
    }
}
