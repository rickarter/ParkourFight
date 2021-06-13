using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject player;

    private BoxCollider2D boxCollider;

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

    void OnCollisionEnter2D(Collision2D other)
    {
        player.transform.position = transform.position;

        Destroy(gameObject);
    }
}
