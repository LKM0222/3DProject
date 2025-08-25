using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [System.Serializable]
    public class PoolConfig
    {
        public Enemy.Type type;
        public GameObject prefab;
        public Transform parent;
        public int initialCount = 10;
    }

    [SerializeField] List<PoolConfig> pools = new List<PoolConfig>();

    readonly Dictionary<Enemy.Type, Queue<GameObject>> poolMap = new Dictionary<Enemy.Type, Queue<GameObject>>();

    void Start()
    {
        Init();
    }

    private void Init()
    {
        foreach (var config in pools)
        {
            if (!poolMap.ContainsKey(config.type))
            {
                poolMap[config.type] = new Queue<GameObject>();
            }

            for (int i = 0; i < Mathf.Max(0, config.initialCount); i++)
            {
                GameObject go = Instantiate(config.prefab, config.parent);
                go.SetActive(false);
                poolMap[config.type].Enqueue(go);
            }
        }
    }

    public void InsertQueue(GameObject _e)
    {
        Enemy enemy = _e.GetComponent<Enemy>();
        if (!poolMap.ContainsKey(enemy.enemyType))
        {
            poolMap[enemy.enemyType] = new Queue<GameObject>();
        }

        poolMap[enemy.enemyType].Enqueue(_e);
        _e.SetActive(false);
        enemy.isChase = false;
    }

    public GameObject GetQueue(Enemy.Type type)
    {
        GameObject resultObj;
        if (!poolMap.ContainsKey(type))
        {
            poolMap[type] = new Queue<GameObject>();
        }

        // 필요 시 자동 확장
        if (poolMap[type].Count == 0)
        {
            var config = pools.Find(p => p.type == type);
            if (config != null && config.prefab != null)
            {
                var created = Instantiate(config.prefab, config.parent);
                created.SetActive(false);
                poolMap[type].Enqueue(created);
            }
            else
            {
                Debug.LogWarning($"[PoolManager] 풀에 해당 타입의 프리팹 설정이 없습니다: {type}");
            }
        }

        resultObj = poolMap[type].Dequeue();
        resultObj.SetActive(true);
        return resultObj;
    }
}
