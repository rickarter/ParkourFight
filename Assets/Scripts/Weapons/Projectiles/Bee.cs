using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour
{
    private HealthBar health;
    private SpriteRenderer spriteRenderer;
    private Material spriteMaterial;
    private new Rigidbody2D rigidbody;

    public float speed = 20;
    public float moveSpeed = 5f;
    public int damage = 5;
    public GameObject target;
    public LayerMask whatIsEnemy;
    public float attackRadius = 7;
    public float sleepTime = 1;
    private float deathTime;
    public float deathDurationTime = .3f;


    private bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<HealthBar>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteMaterial = spriteRenderer.material;
        rigidbody = GetComponent<Rigidbody2D>();
        Invoke(nameof(StartAttack), sleepTime);
    }

    // Update is called once per frame
    void Update()
    {
        if(isAttacking && health.isAlive)
        {
            if(target == null) FindTarget();
            else
            {
                AttackTarget();
            }
        }
        else if(!health.isAlive)
        {
            float t = (Time.time - deathTime) / deathDurationTime;
            if(t > 1 && !GetComponent<AudioSource>().isPlaying) Destroy(gameObject);

            spriteMaterial.SetFloat("_FadeAmount", t);
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        rigidbody.velocity *= 0.5f; 
        rigidbody.gravityScale = 0;
    }

    void FindTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRadius, whatIsEnemy);

        if(colliders.Length == 0) return;

        Collider2D nearestCollider = colliders[0];
        foreach(var collider in colliders)
        {
            if((collider.transform.position-transform.position).magnitude < (nearestCollider.transform.position - transform.position).magnitude)
            {
                nearestCollider = collider;
            }   
        }

        target = nearestCollider.gameObject;
    }

    void AttackTarget()
    {
        //Rotate towards the target
        Vector3 direction = (target.transform.position - transform.position).normalized;
        spriteRenderer.flipY = Vector3.Dot(direction, Vector3.right) < 0;
        transform.right = direction.normalized;

        if(rigidbody.velocity.magnitude > speed) return;
        float multiplier = 1;
        if(Vector3.Dot(direction, rigidbody.velocity) < 0) multiplier = 5;
        rigidbody.AddForce(direction * moveSpeed * multiplier);


    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, attackRadius);
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
        this.health.TakeDamage(100);

        deathTime = Time.time;
        GetComponent<BoxCollider2D>().enabled = false;


    }
}
