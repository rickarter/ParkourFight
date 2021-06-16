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

    private AudioSource audioSource;
    public AudioClip audioClip;

    void Start()
    {
        maxHealth = health;
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        health = (int)Mathf.Clamp(health, 0, Mathf.Infinity);

        // if(this.gameObject.CompareTag("Player"))
        // Debug.Log("took damage" + damage.ToString());

        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void Heal()
    {
        health = maxHealth;
    }
}
