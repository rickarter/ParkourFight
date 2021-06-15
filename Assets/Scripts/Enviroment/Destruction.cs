using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruction : MonoBehaviour
{
    public GameObject particles;

    public void Destruct()
    {
        Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
