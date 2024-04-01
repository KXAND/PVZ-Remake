using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
namespace Zombie
{
    // 对僵尸对象池的简单封装
    public class ZombiesPools : MonoBehaviour
    {
        public static ZombiesPools Instance { get; private set; }

        public Dictionary<ZombieType, ObjectPool<GameObject>> pools = new();
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }
        public void Init(LevelConfig config)
        {
            foreach (var pool in config.zombieList)
            {
                pools.Add(pool.zombieType, new ObjectPool<GameObject>(
                    createFunc: () =>
                    {
                        var zombie = Instantiate(Resources.LoadAsync<GameObject>("Zombies/" + pool.zombieType.ToString()).asset) as GameObject;
                        zombie.transform.SetParent(transform, worldPositionStays: true);
                        return zombie;
                    },
                    defaultCapacity: 10
                    ));
            }
        }
        public GameObject GetFromPool(ZombieType zombieType, Transform parent = null, ZombieState state = ZombieState.Walk)
        {
            var t = pools[zombieType].Get();
            t.transform.parent = parent;
            t.SetActive(true);
            t.GetComponent<ZombieBase>().Init(state);
            return t;
        }
        public void Release(GameObject gameObject, ZombieType poolName)
        {
            pools[poolName].Release(gameObject);
            gameObject.GetComponent<ZombieBase>();
            gameObject.SetActive(false);

        }
    }
}