using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    public float lifeTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(DestroyPig), lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DestroyPig()
    {
        Destroy(gameObject);
    }
}
