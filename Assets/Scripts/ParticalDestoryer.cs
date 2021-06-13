using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticalDestoryer : MonoBehaviour
{
    void Start()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        float totalDuration = particleSystem.startLifetime + particleSystem.main.duration;
        Destroy(gameObject, totalDuration);
    }
}
