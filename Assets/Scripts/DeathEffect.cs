using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    [SerializeField] private float destroyDelay = 1f;
    private ParticleSystem deathParticles;

    private void Start()
    {
        deathParticles = GetComponentInChildren<ParticleSystem>();

        if (deathParticles == null)
        {
            Debug.LogError("No ParticleSystem found in children!");
        }
    }

    public void PlayDeathEffect()
    {
        if (deathParticles != null)
        {
            deathParticles.Play();
            Destroy(gameObject, destroyDelay);
        }
    }
}
