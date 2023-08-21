using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : SingletonMonoBehaviour<ObjectPooling>
{
    public Pool[] pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    [Serializable]
    public struct Pool
    {
        public int size;
        public GameObject prefab;
    }

    public void Start()
    {
        // Initialize Object Pool
        foreach (Pool pool in pools)
        {
            string key = pool.prefab.name;

            CreatePool(key, pool.prefab);

            for (int i = 0; i < pool.size; i++)
                AddGameObject(key, pool.prefab);
        }
    }

    public GameObject GetGameObject(GameObject prefab)
    {
        string key = prefab.name;

        // If no such pool, create pool
        if (!poolDictionary.ContainsKey(key))
            CreatePool(key, prefab);

        // If no prefab can use, create new prefab
        if (poolDictionary[key].Count == 0)
            AddGameObject(key, prefab);

        GameObject currentGameObject = poolDictionary[key].Dequeue();

        currentGameObject.SetActive(true);

        return currentGameObject;
    }

    public void CreatePool(string key, GameObject prefab)
    {

            poolDictionary.Add(key, new Queue<GameObject>());

            GameObject newAnchor = new GameObject(prefab.name + " Anchor");

            newAnchor.transform.SetParent(gameObject.transform);
    }

    public void AddGameObject(string key, GameObject prefab)
    {
        GameObject newGameObject = Instantiate(prefab);

        GameObject anchor = GameObject.Find(prefab.name + " Anchor");

        newGameObject.transform.SetParent(anchor.transform);

        poolDictionary[key].Enqueue(newGameObject);

        newGameObject.SetActive(false);
    }

    public void PushGameObject(GameObject prefab)
    {
        string key = prefab.name.Replace("(Clone)", string.Empty);

        poolDictionary[key].Enqueue(prefab);

        prefab.SetActive(false);
    }
}
