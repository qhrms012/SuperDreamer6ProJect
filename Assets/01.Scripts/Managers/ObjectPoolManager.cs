using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolKey
{
    PlayerBullet,
    EnemyBullet,
    Enemy,
    BlackHole,
    EnemyBlackHole,
    FreezeArrow,
    EnemyFreezeArrow
}

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [System.Serializable]
    public class Pool
    {
        public PoolKey key;
        public GameObject prefab;
    }

    public Pool[] poolsConfig;

    private Dictionary<PoolKey, List<GameObject>> poolsDict;

    private void Awake()
    {
        poolsDict = new Dictionary<PoolKey, List<GameObject>>();

        foreach (var config in poolsConfig)
        {
            poolsDict[config.key] = new List<GameObject>();
        }
    }

    public GameObject Get(PoolKey key)
    {
        var list = poolsDict[key];
        GameObject select = null;

        foreach (GameObject item in list)
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                return select;
            }
        }

        var prefab = System.Array.Find(poolsConfig, p => p.key == key).prefab;
        select = Instantiate(prefab, transform);
        list.Add(select);

        return select;
    }
}
