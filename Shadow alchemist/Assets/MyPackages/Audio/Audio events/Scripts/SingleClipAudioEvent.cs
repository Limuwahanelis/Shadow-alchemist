using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio Event/SingleClipEvent")]
public class SingleClipAudioEvent : AudioEvent
{
    public AudioClip audioClip;
    public override void Play(AudioSource audioSource)
    {
        audioSource.clip = audioClip;
        audioSource.volume = volume*(AudioVolumes.Master / 100.0f)*(AudioVolumes.SFX/100.0f);
        audioSource.pitch = pitch;
        if (audioSource.isPlaying) return;
        audioSource.Play();
    }
    public override void Play(AudioSource audioSource, bool overPlay = false)
    {
        audioSource.clip = audioClip;
        audioSource.volume = volume * (AudioVolumes.Master / 100.0f) * (AudioVolumes.SFX / 100.0f);
        audioSource.pitch = pitch;
        if (!overPlay) return;
        audioSource.Play();
    }
}
