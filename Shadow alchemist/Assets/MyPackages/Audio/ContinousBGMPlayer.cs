using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinousBGMPlayer : MonoBehaviour
{
    public AudioEvent MusicToPlay=>_musicToPlay;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioEvent _musicToPlay;
    public static ContinousBGMPlayer instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            AudioVolumes.OnMasterVolumeChanged += UpdateVolume;
            AudioVolumes.OnBGMVolumeChanged += UpdateVolume;
            DontDestroyOnLoad(gameObject.transform.parent);
        }
        else
        {
            if (instance.MusicToPlay != _musicToPlay)
            {
                Destroy(instance.gameObject);
                DontDestroyOnLoad(gameObject.transform.parent);
                instance = this;
                AudioVolumes.OnMasterVolumeChanged += UpdateVolume;
                AudioVolumes.OnBGMVolumeChanged += UpdateVolume;
            }
            else
            {
                Destroy(gameObject);
                Destroy(gameObject.transform.parent.gameObject);
            }
          
        }
    }
    private void Start()
    {
        if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = true;
        Play();
    }
    public void Play()
    {
        _musicToPlay.Play(_audioSource);
    }
    private void UpdateVolume(int volume)
    {
        _audioSource.volume = (AudioVolumes.Master / 100.0f) * (AudioVolumes.BGM / 100.0f) * _musicToPlay.volume;
    }
    private void OnValidate()
    {
        if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
    }
    private void OnDestroy()
    {
        AudioVolumes.OnMasterVolumeChanged -= UpdateVolume;
        AudioVolumes.OnBGMVolumeChanged -= UpdateVolume;
    }
}
