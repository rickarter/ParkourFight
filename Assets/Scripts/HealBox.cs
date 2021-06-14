using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBox : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D other)
    {
        if(!other.gameObject.CompareTag("Player")) return;

        other.gameObject.GetComponent<HealthBar>().Heal();

        Destroy(gameObject);
    }
}
