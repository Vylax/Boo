using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Player : MonoBehaviour
{
    //player variables
    public float health = 100f;
    public float maxHealth = 100f;
    public float healthDrainSpeed = 1f; //in hp per sec
    private float startTime;

    //post processing variables
    public PostProcessVolume mVolume;
    private Vignette mVignette;

    public float period; //length in seconds of a cycle
    public float vignetteScaleFactor; //how much scaling applies at most during a cycle

    public bool isAlive = true;

    public GameObject audioPrefab;

    public float ghostDamages = 30f;

    private static Player _instance;

    public static Player Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        //get the vignette element
        for (int i = 0; i < mVolume.profile.settings.Count; i++)
        {
            //print(mVolume.profile.settings[i]);
            if (mVolume.profile.settings[i].name.Contains("Vignette"))
            {
                mVignette = (Vignette)mVolume.profile.settings[i];
                break;
            }
        }

        startTime = Time.time;
    }

    private void FixedUpdate()
    {
        RefreshVignette();
        AlterHealth(Time.fixedDeltaTime * -healthDrainSpeed);
    }

    public void Attacked(Ghost ghost)
    {
        AlterHealth(-ghostDamages);
        //do some stuff
        GameManager.Instance.SpawnGhost();

        Destroy(ghost.gameObject);
    }

    public void PlayAudio(string clip, float delay=0f, float volume=1f)
    {
        StartCoroutine(PlayAudioCoroutine(clip, Camera.main.transform.position, delay, volume));
    }

    public void PlayAudio(string clip, Vector3 pos, float delay = 0f, float volume = 1f)
    {
        StartCoroutine(PlayAudioCoroutine(clip, pos, delay, volume));
    }

    public IEnumerator PlayAudioCoroutine(string clip, Vector3 pos, float delay = 0f, float volume = 1f)
    {
        yield return new WaitForSeconds(delay);
        AudioPlayer audioPlayer = Instantiate(audioPrefab, pos, Quaternion.identity).GetComponent<AudioPlayer>();
        audioPlayer.Initialize(clip, volume);
    }

    public void AlterHealth(float value)
    {
        float newHealth = health + value;
        health = Mathf.Clamp(newHealth, 0f, maxHealth);

        if(newHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //do things here
        float endTime = Time.time;
        float survivedTime = endTime - startTime;

        isAlive = false;
    }

    private void RefreshVignette(float value = -1f)
    {
        if(value >= 0f)
        {
            mVignette.intensity.value = value;
            return;
        }

        float noise = Mathf.Sin(Time.time * 2 * Mathf.PI / period) * vignetteScaleFactor;

        mVignette.intensity.value = Mathf.Min(MapValues(health) + noise, 1f);
    }

    private float MapValues(float x)
    {
        return -8f * Mathf.Pow(10f, -3) * x + 1;
    }
}
