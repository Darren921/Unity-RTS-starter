using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [Serializable]
    private struct PooledObject
    {
        public Projectile projectile;
        public int numToSpawn;
    }

    [SerializeField] private PooledObject[] pools;

    private static readonly Dictionary<string, Queue<Projectile>> pooledObjects = 
        new Dictionary<string, Queue<Projectile>>();

    private void Awake()
    {
          pooledObjects.Clear();

        foreach (var pool in pools) 
        {
            string name = pool.projectile.name;
            Transform parent = new GameObject(name).transform;
            parent.SetParent(transform);
            Queue<Projectile> objectsToSpawn = new(pool.numToSpawn);
            for(int i = 0; i< pool.numToSpawn; i++)
            {
                Projectile rb = Instantiate(pool.projectile, parent);
                gameObject.SetActive(false);
                objectsToSpawn.Enqueue(rb);
            }
            pooledObjects.Add(name, objectsToSpawn);

        }
    }

    public static Projectile Shoot(string name, Vector3 location,Quaternion rotation)
    {
        if (!pooledObjects.ContainsKey(name))
        {
            Debug.LogAssertion("Does not conatin key:" + name);
            return null;
        }
        Projectile rb = pooledObjects[name].Dequeue();
        rb.transform.SetLocalPositionAndRotation(location, rotation);
        rb.gameObject.SetActive(true);
        pooledObjects[name].Enqueue(rb);
        return rb;
    }
}
