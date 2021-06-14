using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFlipper : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        spriteRenderer.flipX = rigidbody.velocity.x < 0;
    }
}
