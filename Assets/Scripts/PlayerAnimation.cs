using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    //Components
    public Animator animator;
    private Rigidbody2D rb;

    //Asignables
    public float multiplier = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
    }

    void Animate()
    {
        
    }
}
