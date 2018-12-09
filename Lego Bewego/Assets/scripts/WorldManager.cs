using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    private static WorldManager instance;

    public List<GameObject> mines;
    public GameObjectPool minePool;
    public List<GameObject> currentMines;

    public List<GameObject> groupies;
    public GameObjectPool groupyPool;
    public List<GameObject> currentGroupies;


    public List<GameObject> islandTiles;
    public List<GameObject> currentIslandTiles;
    private GameObjectPool islandPool;

    public List<GameObject> borderTiles;
    private List<GameObject> currentBorderTiles;
    private GameObjectPool borderPool;

    public int levelHeight = 50;
    public float islandAngle = 20;
    private int lastBorderTilePos = 0;
    private float nextIslandPosition = 0;
    public int tileHight = 10;
    public int visibleDistance = 2;
    public int minIslandDistance = 10;

    private float currentPos;

    public static WorldManager getInstance()
    {
        return instance;
    }

    void Awake()
    {
        currentBorderTiles = new List<GameObject>();
        currentMines = new List<GameObject>();
        groupyPool = new GameObjectPool(groupies, this.gameObject, 15);
        borderPool = new GameObjectPool(borderTiles, this.gameObject,10);
        islandPool = new GameObjectPool(islandTiles, this.gameObject,10);
        minePool = new GameObjectPool(mines, this.gameObject, 1);
        
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
	void Update () {
        currentPos = Camera.main.transform.position.x;

        if (currentPos - currentBorderTiles[0].transform.position.x > visibleDistance)
        {
            addBorderTileToWorld();
            removeBorderTileFromWorld();            
        }

        if (currentPos - currentIslandTiles[0].transform.position.x > visibleDistance)
        {
            addIslandToWorld();
            removeIslandFromWorld();
        }

        if(currentPos-currentMines[0].transform.position.x > visibleDistance)
        {
            
            minePool.add(currentMines[0]);
            currentMines.RemoveAt(0);
        }
    }

    private void removeBorderTileFromWorld()
    {
        borderPool.add(currentBorderTiles[0]);
        borderPool.add(currentBorderTiles[1]);
        currentBorderTiles.RemoveAt(0);
        currentBorderTiles.RemoveAt(0);
    }

    private void addBorderTileToWorld()
    {
        GameObject tileUp = borderPool.get();
        GameObject tileDown = borderPool.get();

        currentBorderTiles.Add(tileUp);
        currentBorderTiles.Add(tileDown);

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
        go.transform.position = new Vector3(Mathf.Max(nextIslandPosition + island.dimensions.x/2,visibleDistance), 0, UnityEngine.Random.Range(-(levelHeight-20 - island.dimensions.z) / 2, (levelHeight-20 - island.dimensions.z) / 2));
        go.transform.rotation = Quaternion.Euler(-90, UnityEngine.Random.Range(-islandAngle, islandAngle), 180);
        currentIslandTiles.Add(go);
        island.placeInteracts();
        nextIslandPosition += island.dimensions.x + UnityEngine.Random.Range(0,30) + minIslandDistance;

    }

    private void removeIslandFromWorld()
    {
        islandPool.add(currentIslandTiles[0]);
        currentIslandTiles.RemoveAt(0);
    }
}
