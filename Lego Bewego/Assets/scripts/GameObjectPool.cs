using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool{

    private List<GameObject> pool;
    public List<GameObject> active;
    private List<GameObject> assets;
    private GameObject root;

    public GameObjectPool(List<GameObject> pAssets, GameObject pRoot,int count)
    {
        pool = new List<GameObject>();
        active = new List<GameObject>();
        assets = pAssets;
        root = pRoot;

        for (int i = 0; i < count; i++)
            createGameObject();
    }

    private GameObject createGameObject()
    {
        GameObject go = GameObject.Instantiate(assets[UnityEngine.Random.Range(0, assets.Count)],root.transform);
        pool.Add(go);
        go.SetActive(false);
        return go;
    }

    public GameObject get()
    {
        GameObject go = null;

        if (pool.Count == 0)
            createGameObject();

        int index = UnityEngine.Random.Range(0, pool.Count);
        go = pool[index];
        pool.RemoveAt(index);
        active.Add(go);
        go.SetActive(true);
        return go;
    }

    public void add(GameObject go)
    {
        pool.Add(go);
        go.SetActive(false);
        active.Remove(go);
    }
}
