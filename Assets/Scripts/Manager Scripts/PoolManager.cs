//@Author: Teodor Tysklind / FutureGames / Teodor.Tysklind@FutureGames.nu

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class PoolManager : MonoBehaviour
{
    private static PoolManager instance;
    private int defaultPoolsize = 100;

    public static PoolManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PoolManager>();
            }

            return instance;
        }
    }
    
    private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();

    public void CreatePool(GameObject prefab, int poolSize)
    {
        int poolKey = prefab.GetInstanceID();
        
        Assert.IsFalse(poolDictionary.ContainsKey((poolKey)));

        poolDictionary.Add(poolKey, new Queue<GameObject>());

        for (int i = 0; i < poolSize; i++)
        {
            GameObject newObject = Instantiate(prefab, gameObject.transform) as GameObject;
            newObject.SetActive(false);
            poolDictionary[poolKey].Enqueue(newObject);
        }
    }

    public void CreatePool(GameObject prefab)
    {
        CreatePool(prefab, defaultPoolsize);
    }

    public GameObject ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();
        Assert.IsTrue(poolDictionary.ContainsKey(poolKey));

        GameObject gameObjectToReuse = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(gameObjectToReuse);
            
        gameObjectToReuse.SetActive(true);
        gameObjectToReuse.transform.position = position;
        gameObjectToReuse.transform.rotation = rotation;

        return gameObjectToReuse;
    }

    public bool PoolExists(GameObject poolKey)
    {
        return poolDictionary.ContainsKey(poolKey.GetInstanceID());
    }
}