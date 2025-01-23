using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    [SerializeField] public ParticleSystem deathParticles;
    [SerializeField] private float destroyDelay = 1f; // Durée avant destruction

    private void Start()
{
    // Si les particules ne sont pas assignées, on les crée
    if (deathParticles == null)
    {
        // Créer un nouveau GameObject pour les particules
        var go = new GameObject("DeathParticles");
        go.transform.parent = transform;
        deathParticles = go.AddComponent<ParticleSystem>();
        
        // Désactiver le système de particules au départ
        deathParticles.Stop();
    }
}

    public void PlayDeathEffect()
    {
        // Instantier d'abord le prefab
        GameObject effectInstance = Instantiate(gameObject, transform.position, transform.rotation);
        
        // Optionnel : Si vous voulez que l'effet se détruise automatiquement après un certain temps
        Destroy(effectInstance, 2f); // 2f est la durée en secondes avant destruction
        
        // Si vous devez détruire l'objet original
        Destroy(gameObject); // Ceci détruira l'instance, pas le prefab
    }
}