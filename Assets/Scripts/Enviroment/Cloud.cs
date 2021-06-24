using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float minTime = 10f;
    public float maxTime = 60f;

    public Vector3 destination;

    public float maxOffset;

    private float time;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        float randomScale = Random.Range(.4f, 1.2f);
        transform.localScale = Vector3.one * randomScale;

        transform.position += Vector3.down * Random.Range(0, maxOffset);

        startPosition = transform.position;
        destination.y = startPosition.y;
        
        time = Random.Range(minTime, maxTime);
    }


    private float passedTime = 0;
    // Update is called once per frame
    void Update()
    {
        passedTime += Time.deltaTime;

        float t  = passedTime / time;
        
        transform.position = Vector3.Lerp(startPosition, destination, t);

        if(t >= 1) Destroy(gameObject);
    }
}
