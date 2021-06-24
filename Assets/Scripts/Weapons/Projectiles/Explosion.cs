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
    public GameObject destructionParticle;

    private BoxCollider2D boxCollider;

    private bool collided = false;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        if(boxCollider == null) return;

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

        var i = 0;

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
                    Debug.Log(health.gameObject.name);
                    Debug.Log(Time.time);
                    Debug.Log(i);
                }
                if(collider.transform.TryGetComponent<Destruction>(out var destruction))
                {
                    destruction.Destruct();
                }
            }
            i++;
        }
        GameObject.FindObjectOfType<CameraShake>().StartShaking();
        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
