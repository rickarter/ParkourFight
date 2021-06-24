using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFlipper : MonoBehaviour
{
    public bool flipX = true;

    private new Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;

    private Vector3 scale;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        scale = transform.localScale;
    }

    void FixedUpdate()
    {
        if(flipX)
            spriteRenderer.flipX = rigidbody.velocity.x < 0;
        else
            transform.localScale = new Vector3(scale.x, rigidbody.velocity.x < 0 ? -scale.y : scale.y, scale.z);
    }
}
