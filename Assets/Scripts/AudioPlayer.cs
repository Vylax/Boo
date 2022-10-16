using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class AudioPlayer : MonoBehaviour
{
    private AudioSource source;

    private void Awake()
    {
        source = this.GetComponent<AudioSource>();
    }

    public void Initialize(string clip, float volume = 1f)
    {
        source.clip = Resources.Load<AudioClip>(clip);
        source.volume = volume;
        source.Play();

        Destroy(this.gameObject, source.clip.length);
    }
}
