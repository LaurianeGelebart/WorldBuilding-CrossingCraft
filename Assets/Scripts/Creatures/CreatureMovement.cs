using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contrôle les mouvements et comportements d'une créature dans l'environnement de jeu
/// </summary>
public class CreatureMovement : MonoBehaviour
{

    private Creature associatedCreature; // Créature associée à ce script de mouvement

    public float moveSpeed; // Vitesse de déplacement de la créature

    private GameObject currentTarget; // Alimentaire cible de la créature

    /// <summary>
    /// Initialise le script de mouvement avec une créature spécifique
    /// </summary>
    /// <param name="creature">La créature à associer au script de mouvement</param>
    public void Initialize(Creature creature)
    {
        associatedCreature = creature;
        moveSpeed = creature.Speed;
    }

    /// <summary>
    /// Mise à jour frame par frame du comportement de recherche de nourriture
    /// </summary>
    void Update()
    {
        // Afficher l'état de la faim de la créature
        DisplayHungerBar();

        // Rechercher de la nourriture si la créature n'est pas complètement rassasiée
        if (associatedCreature.faim < 100f)
        {
            // Trouver la nourriture la plus proche si aucune cible n'est définie
            if (currentTarget == null)
            {
                FindNearestFood();
            }

            // Se déplacer vers la nourriture si une cible existe
            if (currentTarget != null)
            {
                MoveTowardsTarget();
            }
        }
    }

    /// <summary>
    /// Affiche une représentation visuelle du niveau de faim dans la console
    /// </summary>
    void DisplayHungerBar()
    {
        // Calculer la longueur de la barre de faim
        int hungerBarLength = 20;
        int filledLength = Mathf.RoundToInt((associatedCreature.faim / 100f) * hungerBarLength);

        // Créer la barre de faim
        string hungerBar = "[";
        for (int i = 0; i < hungerBarLength; i++)
        {
            hungerBar += i < filledLength ? "=" : " ";
        }
        hungerBar += "]";
    }

    /// <summary>
    /// Recherche la nourriture la plus proche adaptée au type de créature
    /// </summary>
    void FindNearestFood()
    {
        // Collecter les prefabs de nourriture compatibles
        List<GameObject> prefabs = new();
        prefabs.AddRange(GameObject.FindGameObjectsWithTag("FoodBoth"));
        if (associatedCreature.Type == CreatureType.Forest)
        {
            prefabs.AddRange(GameObject.FindGameObjectsWithTag("FoodForest"));
        }
        if (associatedCreature.Type == CreatureType.Desert)
        {
            prefabs.AddRange(GameObject.FindGameObjectsWithTag("FoodDesert"));
        }
        float closestDistance = Mathf.Infinity;

        // Trouver la nourriture la plus proche
        foreach (GameObject prefab in prefabs)
        {
            float distance = Vector3.Distance(transform.position, prefab.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentTarget = prefab;
            }
        }
    }

    /// <summary>
    /// Déplace la créature vers sa cible alimentaire
    /// </summary>
    void MoveTowardsTarget()
    {
        if (currentTarget == null) return;

        // Ajuster la hauteur de la cible pour un mouvement horizontal
        Vector3 targetAtEyeLevel = new(
            currentTarget.transform.position.x,
            transform.position.y,
            currentTarget.transform.position.z
        );

        // Calculer la direction vers la cible
        Vector3 directionToTarget = (targetAtEyeLevel - transform.position).normalized;

        // Déplacer la créature vers la cible
        transform.position += directionToTarget * moveSpeed * Time.deltaTime;

        // Orienter la créature dans la direction du mouvement
        if (directionToTarget != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToTarget);
        }

        // Vérifier si la créature est assez proche pour "manger" la nourriture
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (distanceToTarget < 0.5f)
        {
            // Augmenter la satiété de la créature
            if (associatedCreature != null)
            {
                associatedCreature.Eat(50f);
            }

            // Détruire l'objet de nourriture
            Destroy(currentTarget);
            currentTarget = null;
        }
    }

    /// <summary>
    /// Gère les interactions de la créature avec les objets de nourriture
    /// </summary>
    /// <param name="other">Collider de l'objet entrant en collision</param>
    void OnTriggerEnter(Collider other)
    {
        FoodItem foodItem = other.GetComponent<FoodItem>();

        if (foodItem != null)
        {
            if (foodItem.poisonIntensity > 0)
            {
                // Gestion de la nourriture empoisonnée
                associatedCreature.Eat(-foodItem.poisonIntensity);
                associatedCreature.pv -= foodItem.poisonIntensity / 3f;
            }
            else
            {
                // Gestion de la nourriture normale
                associatedCreature.Eat(foodItem.nutritionalValue);
            }

            // Détruire la nourriture après consommation
            Destroy(other.gameObject);
        }
    }
}