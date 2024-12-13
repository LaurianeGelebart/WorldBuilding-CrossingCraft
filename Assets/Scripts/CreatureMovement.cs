using UnityEngine;

public class CreatureMovement : MonoBehaviour
{
    private Creature associatedCreature;
    public float moveSpeed;
    private GameObject currentTarget;

    public void Initialize(Creature creature)
    {
        associatedCreature = creature;
        moveSpeed = creature.Speed;
    }

    void Update()
    {
        // Afficher la barre de faim dans la console
        DisplayHungerBar();

        // Ne chercher de la nourriture que si la faim n'est pas au maximum
        if (associatedCreature.faim < 100f)
        {
            // Find the nearest food if no current target
            if (currentTarget == null)
            {
                FindNearestFood();
            }

            // Move towards the food if a target exists
            if (currentTarget != null)
            {
                MoveTowardsTarget();
            }
        }
    }

    void DisplayHungerBar()
    {
        // Calculer le nombre de caract�res pour la barre de faim
        int hungerBarLength = 20;
        int filledLength = Mathf.RoundToInt((associatedCreature.faim / 100f) * hungerBarLength);

        // Cr�er la barre de faim
        string hungerBar = "[";
        for (int i = 0; i < hungerBarLength; i++)
        {
            hungerBar += i < filledLength ? "=" : " ";
        }
        hungerBar += "]";

        // Afficher dans la console
        Debug.Log($"Creature Hunger: {hungerBar} {associatedCreature.faim:F1}%");
    }

    void FindNearestFood()
    {
        // Chercher tous les prefabs dans la sc�ne
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("Food");
        float closestDistance = Mathf.Infinity;

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

    void MoveTowardsTarget()
    {
        if (currentTarget == null) return;

        // Calculate direction to the target
        Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;

        // Move towards the target
        transform.position += directionToTarget * moveSpeed * Time.deltaTime;

        // Optional: Rotate to face the direction of movement
        if (directionToTarget != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToTarget);
        }

        // Check if close enough to food to "eat" it
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (distanceToTarget < 0.5f)
        {
            // Increase creature's hunger
            if (associatedCreature != null)
            {
                associatedCreature.faim = Mathf.Min(associatedCreature.faim + 50f, 100f); // Cap at 100
            }

            // Destroy the food
            Destroy(currentTarget);
            currentTarget = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        FoodItem foodItem = other.GetComponent<FoodItem>();

        if (foodItem != null)
        {
            if (foodItem.poisonIntensity > 0)
            {
                // Poisonous food
                associatedCreature.faim -= foodItem.poisonIntensity;
                associatedCreature.pv -= foodItem.poisonIntensity / 3f;
            }
            else
            {
                // Normal food
                associatedCreature.faim = Mathf.Min(
                    associatedCreature.faim + foodItem.nutritionalValue,
                    100f
                );
            }

            // Destroy the food after consumption
            Destroy(other.gameObject);
        }
    }
}