using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
        SetIsMuted(DataStorage.IsMuted);
    }

    public void PlaySound(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }

    public void SetIsMuted(bool isMuted)
    {
        source.volume = isMuted ? 0 : 1;
    }
}
