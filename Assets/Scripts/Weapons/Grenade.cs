using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Weapon
{
    protected bool throwing = false;
    public float throwForce;

    void Start()
    {
        player = transform.parent.GetComponent<PlayerScript>();
        player.weapon = this;
        scale = transform.localScale;
    }

    public override void Fire(MyInput input)
    {
        player.playerAnimation.ThrowAnimation();
        Invoke(nameof(ThrowGrenade), player.playerAnimation.throwAnimationTime * 0.8f);

        base.Fire(input);
    }

    public override void Rotate(Vector2 direction)
    {
        Vector3 shoulderOffset = player.playerMovement.shoulderOffset;
        shoulderOffset = transform.up * shoulderOffset.y + transform.right * shoulderOffset.x;

        Vector3 shoulderPos = shoulderOffset + player.transform.position;
        Vector3 handPos = !player.playerAnimation.frontArmEffectorO.parent.gameObject.activeSelf ? player.playerAnimation.frontArmEffector.position : player.playerAnimation.frontArmEffectorO.position;

        Vector3 distance = handPos - shoulderPos;
        Debug.DrawLine(shoulderPos, handPos);
        if(distance.magnitude > player.playerMovement.grabRadius)
            distance = distance.normalized * player.playerMovement.grabRadius;
        transform.position = shoulderPos + distance;
    }

    void Update()
    {
        ToggleThrowing(player.playerInput.input.fire > 0);
    }

    void FixedUpdate()
    {
        Rotate(player.playerInput.input.aim);

        if(!throwing  || !readyToShoot) return;
        Fire(player.playerInput.input);
    }

    void ThrowGrenade()
    {
        Vector2 direction = player.playerInput.input.aim;
        if(direction.x == 0 && direction.y == 0) direction = player.rigidBody.velocity.normalized;
        GameObject grenade = Instantiate(projectile, transform.position, Quaternion.identity);
        grenade.GetComponent<Rigidbody2D>().velocity = player.rigidBody.velocity;
        grenade.GetComponent<Rigidbody2D>().AddForce(throwForce * direction);
    }

    private bool hasChanged;
    void ToggleThrowing(bool throwing)
    {
        if(throwing)
        {
            if(!hasChanged) 
            {
                this.throwing = true;
                hasChanged = true;

                float delay = 0.1f;
                Invoke(nameof(ResetThrowing), delay);
            }
        }
        else
        {
            hasChanged = false;
        }
    }

    void ResetThrowing()
    {
        throwing = false;
    }
}
