using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsSpawner : MonoBehaviour
{
    public GameObject cloud;

    public Transform start;
    public Transform end;

    public float spawnTimeMin = 0.1f;
    public float spawnTimeMax = 1f;

    // Start is called before the first frame update
    void Start()
    {
        SpawnCloud();
    }

    void SpawnCloud()
    {
        Invoke(nameof(SpawnCloud), Random.Range(spawnTimeMin, spawnTimeMax));

        GameObject cloud = Instantiate(this.cloud, start.position, Quaternion.identity);
        cloud.GetComponent<Cloud>().destination = end.position;
    }
}
