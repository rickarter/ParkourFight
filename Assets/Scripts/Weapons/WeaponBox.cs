using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBox : MonoBehaviour
{
    public List<GameObject> weapons;

    void OnCollisionEnter2D(Collision2D other)
    {
        if(!other.gameObject.CompareTag("Player")) return;

        if(other.gameObject.GetComponent<PlayerScript>().weapon != null)
            Destroy(other.gameObject.GetComponent<PlayerScript>().weapon.gameObject);

            IEnumerator coroutine = GiveWeapon(other);
            StartCoroutine(coroutine);
    }

    IEnumerator GiveWeapon(Collision2D player)
    {
        yield return 1;

        int slot = Random.Range(0, weapons.Count);
        GameObject weapon = Instantiate(weapons[slot], player.transform);
        Destroy(gameObject);
    }
}