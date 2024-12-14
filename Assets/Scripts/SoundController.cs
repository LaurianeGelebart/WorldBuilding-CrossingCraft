using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioClip bornSound;
    public AudioClip eatingSound;
    public AudioClip deathSound;

    // Reference to the AudioSource component
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayBornSound()
    {
        if (bornSound != null)
        {
            _audioSource.PlayOneShot(bornSound);
        }
    }

    public void PlayEatingSound()
    {
        if (eatingSound != null)
        {
            _audioSource.PlayOneShot(eatingSound);
        }
    }
    
    public void PlayDeathSound()
    {
        if (deathSound != null)
        {
            _audioSource.PlayOneShot(deathSound);
        }
    }
}