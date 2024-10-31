using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio Event/BGMClipEvent")]
public class BGMAudioEvent : AudioEvent
{
    public AudioClip audioClip;
    public override void Play(AudioSource audioSource)
    {
        audioSource.clip = audioClip;
        audioSource.volume = volume * (AudioVolumes.Master / 100.0f) * (AudioVolumes.BGM / 100.0f);
        audioSource.pitch = pitch;
        if (audioSource.isPlaying) return;
        audioSource.Play();
    }
    public override void Play(AudioSource audioSource, bool overPlay = false)
    {
        audioSource.clip = audioClip;
        audioSource.volume = volume * (AudioVolumes.Master / 100.0f) * (AudioVolumes.BGM / 100.0f);
        audioSource.pitch = pitch;
        if (!overPlay) return;
        audioSource.Play();
    }
    public override void Preview(AudioSource audioSource, float masterVol, float multVol)
    {
        audioSource.clip = audioClip;
        audioSource.volume = volume * (masterVol / 100.0f) * (multVol / 100.0f);
        audioSource.pitch = pitch;
        if (audioSource.isPlaying) return;
        audioSource.Play();
    }
}
