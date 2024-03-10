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
}
