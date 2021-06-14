using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionForce = 1000f;
    public float explostionRadius = 5f;

    public LayerMask whatIsTrigger;

    public int damage = 30;

    public GameObject particle;

    private BoxCollider2D boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;

        Invoke(nameof(EnableCollider), 0.125f);
    }

    void EnableCollider()
    {
        boxCollider.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        int layer = other.gameObject.layer;
        if (whatIsTrigger != (whatIsTrigger | (1 << layer))) return;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explostionRadius);

        foreach(Collider2D collider in colliders)
        {
            if(collider.gameObject != gameObject)
            {
                Vector2 direction = (collider.transform.position - transform.position).normalized;
                RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + direction * 0.6f, direction);

                if(hit.rigidbody)
                {
                    hit.rigidbody.AddForce(direction * explosionForce);
                }
                if(hit.transform.TryGetComponent<HealthBar>(out var health))
                {
                    health.TakeDamage(damage);
                }
            }
        }
        GameObject.FindObjectOfType<CameraShake>().StartShaking();
        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
