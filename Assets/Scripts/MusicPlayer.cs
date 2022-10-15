using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        currSource = 0;
        currClip = 0;
        sources[0].clip = clips[0];
        sources[0].Play();
    }

    //Debug
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeClip();
        }
    }

    public void ChangeClip()
    {
        canFade = false;
        currClip = currClip + 1 < clips.Length ? currClip + 1 : 0;
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
