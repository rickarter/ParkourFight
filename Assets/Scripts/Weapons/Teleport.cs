using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    private GameObject player;

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

    void Update()
    {
        Debug.Log(player);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // if(other.gameObject.CompareTag("Player")) return;

        Debug.Log(this.player);

        Destroy(gameObject);
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
        Debug.Log("is set");
        Debug.Log(this.player);
    }
}
