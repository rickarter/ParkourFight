using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public float health = 100f;
    public bool isAlive
    {
        get
        {
            return health > 0;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, Mathf.Infinity);
    }
}
