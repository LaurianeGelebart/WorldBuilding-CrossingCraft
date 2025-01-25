using UnityEngine;

/// <summary>
/// Contrôle la gestion des sons pour les événements de créature
/// </summary>
public class SoundController : MonoBehaviour 
{
    public AudioClip bornSound;   // Son de naissance
    public AudioClip eatingSound; // Son de consommation
    public AudioClip deathSound;  // Son de mort

    [Range(0f, 1f)]
    public float minDistance = 1f;   // Distance minimale d'écoute
    public float maxDistance = 80f;  // Distance maximale d'écoute
    private float _spatialBlend = 1f;  // Mélange spatial (1 = 3D, 0 = 2D)

    private AudioSource _audioSource;  // Composant de lecture audio

    /// <summary>
    /// Initialise le composant audio au démarrage
    /// </summary>
    void Start()
    {
        // Récupérer ou ajouter un composant AudioSource
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configurer les paramètres audio
        _audioSource.spatialBlend = _spatialBlend;
        _audioSource.minDistance = minDistance;
        _audioSource.maxDistance = maxDistance;
        _audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    /// <summary>
    /// Joue le son de naissance de la créature
    /// </summary>
    public void PlayBornSound()
    {
        if (bornSound != null)
        {
            _audioSource.PlayOneShot(bornSound);
        }
    }

    /// <summary>
    /// Joue le son de consommation de nourriture
    /// </summary>
    public void PlayEatingSound()
    {
        if (eatingSound != null)
        {
            _audioSource.PlayOneShot(eatingSound);
        }
    }

    /// <summary>
    /// Joue le son de mort de la créature
    /// </summary>
    public void PlayDeathSound()
    {
        if (deathSound != null)
        {
            _audioSource.PlayOneShot(deathSound);
        }
    }
}