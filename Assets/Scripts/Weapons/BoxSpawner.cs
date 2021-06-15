using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    public float minTime = 5f;
    public float maxTime = 15f;

    public GameObject weaponBox;
    public GameObject healBox;

    private GameObject currentBox;

    void Start()
    {
        currentBox = null;
        Invoke(nameof(SpawnBox), 1);
    }

    void Update()
    {

    }

    void SpawnBox()
    {
        int value = Random.Range(0, 10);

        GameObject box = weaponBox;
        if(value > 7) box = healBox;


        if(currentBox == null)
            currentBox = Instantiate(box, transform.position, Quaternion.identity);

        float time = Random.Range(minTime, maxTime);
        Invoke(nameof(SpawnBox), time);
    }
}
