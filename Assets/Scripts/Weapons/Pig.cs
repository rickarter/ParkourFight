using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    public float lifeTime = 1f;
    public int damage = 1;
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(DestroyPig), lifeTime);
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
        Invoke(nameof(EnableCollider), 0.25f);
    }

    void EnableCollider()
    {
        boxCollider.enabled = true;
    }

    void DestroyPig()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(!other.gameObject.CompareTag("Player")) return;

        if(other.gameObject.TryGetComponent<HealthBar>(out var health))
        {
            IEnumerator coroutine = Damage(health);
            StartCoroutine(coroutine);
        }
    }

    IEnumerator Damage(HealthBar health)
    {
        yield return 0;

        health.TakeDamage(damage);

        Destroy(gameObject);
    }
}
