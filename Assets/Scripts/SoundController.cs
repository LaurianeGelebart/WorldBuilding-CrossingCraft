using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioClip bornSound;
    public AudioClip eatingSound;
    public AudioClip deathSound;
    
    [Range(0f, 1f)]
    public float minDistance = 1f;
    public float maxDistance = 80f;
    private float _spatialBlend = 1f;  // 1 = 3D, 0 = 2D
    
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        _audioSource.spatialBlend = _spatialBlend;
        _audioSource.minDistance = minDistance;
        _audioSource.maxDistance = maxDistance;
        _audioSource.rolloffMode = AudioRolloffMode.Linear;
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