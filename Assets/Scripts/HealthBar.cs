using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public int health = 100;
    private int maxHealth;
    public bool isAlive
    {
        get
        {
            return health > 0;
        }
    }

    void Start()
    {
        maxHealth = health;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        health = (int)Mathf.Clamp(health, 0, Mathf.Infinity);
    }

    public void Heal()
    {
        health = maxHealth;
    }
}
