using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnderPearl : Grenade
{
    void Start()
    {
        player = transform.parent.GetComponent<PlayerScript>();
        player.weapon = this;
        scale = transform.localScale;
        destoryTime = player.playerAnimation.throwAnimationTime * 0.8f;
    }

    public override void Throw()
    {
        Vector2 direction = player.playerInput.input.aim;
        if(direction.x == 0 && direction.y == 0) direction = player.rigidBody.velocity.normalized;
        GameObject grenade = Instantiate(projectile, transform.position, Quaternion.identity);

        grenade.GetComponent<Teleport>().player = player.gameObject; 
        grenade.GetComponent<Rigidbody2D>().velocity = player.rigidBody.velocity;
        grenade.GetComponent<Rigidbody2D>().AddForce(throwForce * direction);
    }
}
