using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private PlayerScript[] players;
    private bool hasStopped = false;

    //Slowdown
    public float slowdownTime = 1f;
    public float slowdownTimeScale  = 0.5f;
    private float stopTime; 

    //Level transition
    public float invokeNewLevelTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindObjectsOfType<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!players[0].healthBar.isAlive || !players[1].healthBar.isAlive)
        {
            hasStopped = true;
        }

        if(hasStopped)
        {
            float t = (Time.time - stopTime) / slowdownTime;
            Time.timeScale = Mathf.Lerp(Time.timeScale, slowdownTimeScale, t);

            if(t >= 1) Invoke(nameof(LoadRandomLevel), invokeNewLevelTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<HealthBar>().TakeDamage(100);
        }
        else
        {
            Destroy(other.gameObject);
        }
    }

    void LoadRandomLevel()
    {
        Debug.Log("Load new level");
    }
}
