using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField] private CubeConfig _config;
    [SerializeField] private AudioSource _audioSource;

    private void Start()
    {
        if (_config.Audio.BackgroundMusic != null)
        {
            _audioSource.clip = _config.Audio.BackgroundMusic;
        }

        _audioSource.Play();

    }
}
