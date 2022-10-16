using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource[] sources;

    public AudioClip[] clips;
    public float[] distances;

    public int currSource;
    public int currClip;

    public float fadeDuration = 2f;

    public bool canFade = true;

    private static MusicPlayer _instance;

    public static MusicPlayer Instance { get { return _instance; } }


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
        currSource = 0;
        //currClip = 0;
        //sources[0].clip = clips[0];
        //sources[0].Play();
    }

    //Debug
    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeClip();
        }
    }*/

    private int queuedStage = -1;
    private bool waiting = false;

    private IEnumerator WaitForFade()
    {
        waiting = true;
        yield return new WaitUntil(() => canFade);
        ChangeClip(queuedStage);
        waiting = false;
    }

    public void ChangeClip(int stage)
    {
        if (!canFade)
        {
            queuedStage = stage;
            if (!waiting)
            {
                StartCoroutine(WaitForFade());
            }
            return;
        }

        canFade = false;
        //currClip = currClip + 1 < clips.Length ? currClip + 1 : 0;
        currClip = stage;
        StartCoroutine(Fade(fadeDuration, currClip, sources[currSource], sources[(currSource + 1) % 2]));
        currSource = (currSource + 1) % 2;
    }

    public IEnumerator Fade(float duration, int audioClip, AudioSource sourceOut, AudioSource sourceIn, float volume = 0.35f)
    {
        float syncTime = sourceOut.time;
        sourceIn.volume = 0f;
        sourceIn.clip = clips[audioClip];
        sourceIn.time = syncTime;
        sourceIn.Play();

        float timeStep = duration / 100f;
        float volumeStep = volume / 100f;

        for (int i = 0; i < 100; i++)
        {
            sourceOut.volume -= volumeStep;
            sourceIn.volume += volumeStep;
            yield return new WaitForSeconds(timeStep);
        }

        sourceOut.Stop();
        canFade = true;
    }
}
