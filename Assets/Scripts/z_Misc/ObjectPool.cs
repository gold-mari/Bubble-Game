using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    // The prefab spawned when we need to add another object.
    private GameObject prefab;
    // The list storing our pool of objects.
    public readonly List<GameObject> pool = new();
    // The transform that we spawn all objects under.
    private Transform spawnParent;

    public ObjectPool(GameObject _prefab, Transform _spawnParent, int initialCapacity=0)
    {
        prefab = _prefab;
        spawnParent = _spawnParent;

        // Fill in the pool to the intial capacity.
        for (int i=0; i<initialCapacity; i++) {
            GameObject obj = GameObject.Instantiate(prefab, spawnParent);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject Request()
    {
        // Requests an inactive object from our pool, or makes one if it
        // doesn't exist.
        // ================

        // Find the first inactive object in our pool.
        GameObject obj = pool.Find(x => !x.activeInHierarchy);
        
        if (obj != null) {
            // If it exists, set it active.
            obj.SetActive(true);
        } else {
            // If none are inactive, make a new one and return it.
            obj = GameObject.Instantiate(prefab, spawnParent);
            pool.Add(obj);
        }

        return obj;
    }

    public void Deactivate(GameObject obj)
    {
        // Deactivates an object in our pool.
        // ================

        if (!pool.Contains(obj)) {
            Debug.LogError("ObjectPool Error: Deactivate failed. Object was not in pool.\n"
                          +"Try using object.SetActive(false)", obj);
        } else {
            obj.SetActive(false);
        }
    }

    public void DeactivateAll()
    {
        // Deactivates all objects in our pool.
        // ================

        foreach (GameObject obj in pool) {
            obj.SetActive(false);
        }
    }
}