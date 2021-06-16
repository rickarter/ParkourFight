using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerMovement), typeof(PlayerAnimation))]
[RequireComponent(typeof(PlayerInput), typeof(PlayerStunts))]
public class PlayerScript : MonoBehaviour
{
    //Components
    public Rigidbody2D rigidBody;
    public PlayerMovement playerMovement;
    public PlayerInput playerInput;
    public PlayerAnimation playerAnimation;
    public PlayerStunts playerStunts;
    public HealthBar healthBar;

    public Weapon weapon;

    void Reset()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerStunts = GetComponent<PlayerStunts>();
        healthBar = GetComponent<HealthBar>();
    }

    void Awake()
    {
        Reset();
    }

    void FixedUpdate()
    {
        MyInput input = playerInput.input;

        playerMovement.Movement(input);
        playerStunts.Stunts(input);
    }
}
