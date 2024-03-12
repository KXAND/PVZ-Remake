using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
namespace Zombie
{
    // �Խ�ʬ����صļ򵥷�װ
    public class ZombiesPools : MonoBehaviour
    {
        public static ZombiesPools Instance { get; private set; }

        public ZombiePoolInfo[] zombiePoolList;
        public Dictionary<string, ObjectPool<GameObject>> pools = new();

        private void Start()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);

            foreach (var info in zombiePoolList)
            {
                pools.Add(info.poolName, new ObjectPool<GameObject>(
                    createFunc: () =>
                    {
                        var zombie = Instantiate(info.prefab);
                        zombie.transform.SetParent(transform, worldPositionStays: true);
                        return zombie;
                    },
                    defaultCapacity: info.quantity
                    ));
            }

        }
        public GameObject GetFromPool(string poolName)
        {
            GameObject t = pools[poolName].Get();

            return t;
        }
        public void Release(GameObject gameObject, string poolName)
        {
            pools[poolName].Release(gameObject);

        }
    }

    // ����Ӧ��ʹ��Scriptable...
    [System.Serializable]
    public struct ZombiePoolInfo
    {
        public string poolName;
        public int quantity;
        public GameObject prefab;
    }
}