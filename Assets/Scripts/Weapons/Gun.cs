using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    public override void Fire(MyInput input)
    {
        if(input.fire == 0) return;

        GameObject projectile = Instantiate(this.projectile, tip.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().AddForce(transform.right * shootForce);
        player.rigidBody.AddForce(-transform.right * recoil);

        GameObject.FindObjectOfType<CameraShake>().StartShaking();

        base.Fire(input);
    }

    public override void Rotate(Vector2 direction)
    {
        if(direction.x != 0 && direction.y != 0)
            transform.right = direction;
            Vector3 parentScale = transform.parent.localScale;
            transform.localScale = new Vector3(
                Mathf.Sign(parentScale.x) * scale.x,
                Mathf.Sign(parentScale.x) * scale.y,
                Mathf.Sign(parentScale.z) * scale.z
            );
    }
}
