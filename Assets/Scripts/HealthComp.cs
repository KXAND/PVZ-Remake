using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComp : MonoBehaviour
{
    [SerializeField] float health;

    public void Init(float health)
    {
        this.health = health;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0)
        { Destroy(this.gameObject); }
    }

    /*
     * 植物
     * 1. takedamage：发光、声音
     * 2. 死后播放oau！
     * 僵尸
     * 1. takedamage：声音
     * 2. 根据状态不同切换到不同的状态
     */

}
