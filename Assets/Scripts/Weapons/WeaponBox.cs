using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBox : MonoBehaviour
{
    public List<GameObject> weapons;

    void OnCollisionEnter2D(Collision2D other)
    {
        /*if(!other.gameObject.CompareTag("Player")) return;

        if(other.gameObject.GetComponent<PlayerScript>().weapon != null)
            Destroy(other.gameObject.GetComponent<PlayerScript>().weapon.gameObject);

        IEnumerator coroutine = GiveWeapon(other);
        StartCoroutine(coroutine);*/

        if(other.gameObject.TryGetComponent<PlayerScript>(out var player))
        {
            if(player.weapon != null)
                Destroy(player.weapon.gameObject);

            Debug.Log("-------");
            Debug.Log(player.gameObject);
            IEnumerator coroutine = GiveWeapon(player.gameObject);
            StartCoroutine(coroutine);
        }
    }

    IEnumerator GiveWeapon(GameObject player)
    {
        yield return 0;

        int slot = Random.Range(0, weapons.Count);
        GameObject weapon = Instantiate(weapons[slot], player.transform);
        Debug.Log(player.gameObject);
        Destroy(gameObject);
    }
}