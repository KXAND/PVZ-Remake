using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New LevelConfig", menuName = "Scriptable Objects/New LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [System.Serializable]
    public struct ZombieList
    {
        public ZombieType zombieType;
        [Range(1, 999)]
        public int quantity;
    }

    public ZombieList[] zombieList;

    [Range(1, 5)]
    public int flags;

}
