﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    private static WorldManager instance;

    public Transform islandTransform;
    public Transform groupieTransform;
    public Transform borderTransform;
    public Transform minesTransform;

    public List<GameObject> mines;
    public GameObjectPool minePool;

    public List<GameObject> groupies;
    public GameObjectPool groupyPool;

    public List<GameObject> islandTiles;
    private GameObjectPool islandPool;

    public List<GameObject> borderTiles;
    private GameObjectPool borderPool;

    public int levelHeight = 50;
    public float islandAngle = 20;
    private int lastBorderTilePos = 0;
    private float nextIslandPosition = 0;
    public int tileHight = 10;
    public int visibleDistance = 2;
    public int minIslandDistance = 10;

    public static WorldManager getInstance()
    {
        return instance;
    }

    void Awake()
    {
        groupyPool = new GameObjectPool(groupies, groupieTransform.gameObject, 0);
        borderPool = new GameObjectPool(borderTiles, borderTransform.gameObject,0);
        islandPool = new GameObjectPool(islandTiles, islandTransform.gameObject,0);
        minePool = new GameObjectPool(mines, minesTransform.gameObject, 0);
        
        if(instance == null)
        {
            instance = this;
        }
    }
    
	// Use this for initialization
	void Start () {
        lastBorderTilePos = -visibleDistance;
        nextIslandPosition = visibleDistance;

        while (lastBorderTilePos < visibleDistance)
        {
            addBorderTileToWorld();
        }

        for(int i = 0; i<5; i++)
        {
            addIslandToWorld();
        }
        
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float deletePos = Camera.main.transform.position.x - visibleDistance;

        if (borderPool.active[0].transform.position.x <deletePos)
        {
            addBorderTileToWorld();
            removeBorderTileFromWorld();            
        }

        foreach (GameObject groupy in groupyPool.active)
        {
            if (groupy.transform.position.x < deletePos)
            {
                groupy.GetComponent<StoneContainer>().reset();
                groupyPool.add(groupy);

                break;
            }
        }


        foreach (GameObject island in islandPool.active)
        {
            if (island.transform.position.x < deletePos)
            {
                islandPool.add(island);
                break;
            }
        }

        foreach (GameObject mine in minePool.active)
        {
            if(mine.transform.position.x < deletePos)
            {
                minePool.add(mine);
                break;
            }
        }

        if (Camera.main.transform.position.x > nextIslandPosition - visibleDistance)
        {
            addIslandToWorld();
        }
    }

    private void clearPool(GameObjectPool pool, float deletePos)
    {
        foreach (GameObject go in pool.active)
        {
            if (go.transform.position.x < deletePos)
            {
                minePool.add(go);
                break;
            }
        }
    }

    private void removeBorderTileFromWorld()
    {
        borderPool.add(borderPool.active[0]);
        borderPool.add(borderPool.active[0]);
    }

    private void addBorderTileToWorld()
    {
        GameObject tileUp = borderPool.get();
        GameObject tileDown = borderPool.get();

        tileDown.transform.position = new Vector3(lastBorderTilePos, -1.0f, -levelHeight / 2.0f);
        tileUp.transform.position = new Vector3(lastBorderTilePos, -1.0f, levelHeight / 2.0f);
        tileUp.transform.rotation = Quaternion.Euler(0, 0, 0);
        tileDown.transform.rotation = Quaternion.Euler(0, 180, 0);

        lastBorderTilePos += tileHight;
    }

    private void addIslandToWorld()
    {
        GameObject go = islandPool.get();
        Island island = go.GetComponent<Island>();
        go.transform.position = new Vector3(Mathf.Max(nextIslandPosition + island.dimensions.x/2,visibleDistance), 0, UnityEngine.Random.Range(-(levelHeight-10 - island.dimensions.z) / 2, (levelHeight-10 - island.dimensions.z) / 2));
        go.transform.rotation = Quaternion.Euler(-90, UnityEngine.Random.Range(-islandAngle, islandAngle), 180);
        island.placeInteracts();
        nextIslandPosition += island.dimensions.x + UnityEngine.Random.Range(0,30) + minIslandDistance;

    }

    private void removeIslandFromWorld()
    {
        islandPool.add(islandPool.active[0]);
    }
}
