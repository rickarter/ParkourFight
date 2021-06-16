using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    private PlayerScript[] players;
    private bool hasStopped = false;

    public float gameStopTime = 0.3f;

    //Slowdown
    public float slowdownTimeScale  = 0.5f;
    private float stopTime; 

    //Post processing
    public Volume volume;

    private Vignette vignette;
    private ChromaticAberration chromaticAberration; 
    private Bloom bloom;

    public float vignetteIntensity = .6f;
    public float chromaticAberrationIntensity = .4f;
    public float bloomIntensity = 5f;

    public float vignetteValue = .15f;
    public float chromaticAberrationValue = .1f;
    public float bloomValue = 1f;

    //Win text
    public RectTransform winTextRectTransform;

    //Level transition
    private bool hasInvoked = false;
    public float invokeNewLevelTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindObjectsOfType<PlayerScript>();
        volume.profile.TryGet<Vignette>(out vignette);
        volume.profile.TryGet<ChromaticAberration>(out chromaticAberration);
        volume.profile.TryGet<Bloom>(out bloom);

        vignette.intensity.value = vignetteValue;
        chromaticAberration.intensity.value = chromaticAberrationValue;
        bloom.intensity.value = bloomValue;
    }

    // Update is called once per frame
    void Update()
    {
        if((!players[0].healthBar.isAlive || !players[1].healthBar.isAlive) && !hasStopped)
        {
            stopTime = Time.time;
            hasStopped = true;

            if(players[0].healthBar.health > players[1].healthBar.health)
            {
                winTextRectTransform.GetComponent<Text>().text = players[0].name + " won";
            }
            else if(players[0].healthBar.health < players[1].healthBar.health)
            {
                winTextRectTransform.GetComponent<Text>().text = players[1].name + " won";
            }
            else
            {
                winTextRectTransform.GetComponent<Text>().text = "Draw";
            }
        }

        if(hasStopped && !hasInvoked)
        {
            float t = (Time.time - stopTime) / gameStopTime;
            Time.timeScale = Mathf.Lerp(1, slowdownTimeScale, t);

            vignette.intensity.value = Mathf.Lerp(vignetteValue, vignetteIntensity, t);
            chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberrationValue, chromaticAberrationIntensity, t);
            bloom.intensity.value = Mathf.Lerp(bloomValue, bloomIntensity, t);

            winTextRectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);

            if(t >= 1) 
            {
                hasInvoked = true;
                Invoke(nameof(LoadRandomLevel), invokeNewLevelTime);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<HealthBar>().TakeDamage(100);
            Debug.Log("UWo");
        }
        else
        {
            Destroy(other.gameObject);
        }
    }

    void LoadRandomLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(Random.Range(0, SceneManager.sceneCountInBuildSettings-1));
    }
}
