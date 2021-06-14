using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeGun : Weapon
{
    public override void Fire(MyInput input)
    {
        if(input.fire == 0) return;

        GameObject projectile = Instantiate(this.projectile, tip.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().AddForce(transform.right * shootForce);
        player.rigidBody.AddForce(-transform.right * recoil);

        base.Fire(input);
    }
}
