using UnityEngine;
using UnityEngine.AI;

public class CreatureMovement : MonoBehaviour
{
    private Creature associatedCreature;
    private NavMeshAgent navMeshAgent;
    private GameObject currentTarget;
    private float lastWanderTime;
    private float wanderInterval = 10f;
    private Vector3 wanderOrigin;

    public void Initialize(Creature creature)
    {
        associatedCreature = creature;

        // Ajouter NavMeshAgent si pas déjà présent
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        }

        // Configuration du NavMeshAgent
        navMeshAgent.speed = creature.Speed;
        navMeshAgent.acceleration = 8f;
        navMeshAgent.angularSpeed = 120f;
        navMeshAgent.stoppingDistance = 0.5f;
        navMeshAgent.radius = 2f * (creature.ScaleFactor / 2);
        navMeshAgent.height = 3f * (creature.ScaleFactor / 2);

        // Vérifier que l'agent est sur le NavMesh
        NavMeshHit hit;
        if (!NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
        {
            Debug.LogWarning($"Creature not on NavMesh at {transform.position}");
        }

        // Set initial wander origin to starting position
        wanderOrigin = transform.position;
    }

    void Update()
    {
        // Display hunger bar
        DisplayHungerBar();

        // Manage creature's state based on hunger
        if (associatedCreature.faim < 100f)
        {
            // Prioritize finding food
            FindAndMoveToFood();
        }
        else
        {
            // When not hungry, wander around
            WanderRandomly();
        }

        // Optional: Update creature's life points
        UpdateLifePoints();
    }

    void FindAndMoveToFood()
    {
        // Vérifier si l'agent NavMesh est valide
        if (navMeshAgent == null || !navMeshAgent.enabled)
        {
            Debug.LogWarning("NavMeshAgent is not properly initialized");
            return;
        }

        // If no current target, find nearest food
        if (currentTarget == null)
        {
            FindNearestFood();
        }

        // Move to food if target exists and on NavMesh
        if (currentTarget != null)
        {
            // Vérifier si la destination est sur le NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(currentTarget.transform.position, out hit, 10f, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
            }
            else
            {
                Debug.LogWarning($"Food target at {currentTarget.transform.position} is not on NavMesh");
                currentTarget = null;
            }

            // Check if reached food
            if (Vector3.Distance(transform.position, currentTarget.transform.position) <= navMeshAgent.stoppingDistance)
            {
                ConsumeFood();
            }
        }
    }

    void FindNearestFood()
    {
        GameObject[] foodItems = GameObject.FindGameObjectsWithTag("Food");
        float closestDistance = Mathf.Infinity;

        foreach (GameObject foodItem in foodItems)
        {
            float distance = Vector3.Distance(transform.position, foodItem.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentTarget = foodItem;
            }
        }
    }

    void ConsumeFood()
    {
        if (currentTarget == null) return;

        FoodItem foodItem = currentTarget.GetComponent<FoodItem>();
        if (foodItem != null)
        {
            if (foodItem.poisonIntensity > 0)
            {
                // Poisonous food affects hunger and health
                associatedCreature.faim -= foodItem.poisonIntensity;
                associatedCreature.pv -= foodItem.poisonIntensity / 3f;
            }
            else
            {
                // Normal food increases hunger
                associatedCreature.faim = Mathf.Min(
                    associatedCreature.faim + foodItem.nutritionalValue,
                    100f
                );
            }

            // Destroy the food
            Destroy(currentTarget);
            currentTarget = null;
        }
    }

    void WanderRandomly()
    {
        // Check if it's time to wander
        if (Time.time - lastWanderTime > wanderInterval)
        {
            // Generate a random point near the origin
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection += wanderOrigin;

            NavMeshHit hit;
            Vector3 finalPosition;
            if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
            {
                finalPosition = hit.position;
                navMeshAgent.SetDestination(finalPosition);
                lastWanderTime = Time.time;
            }
        }
    }

    void UpdateLifePoints()
    {
        // Gradually decrease life points
        associatedCreature.UpdatePv();

        // Optional: Die if life points reach 0
        if (associatedCreature.pv <= 0)
        {
            associatedCreature.Die();
        }
    }

    void DisplayHungerBar()
    {
        int hungerBarLength = 20;
        int filledLength = Mathf.RoundToInt((associatedCreature.faim / 100f) * hungerBarLength);

        string hungerBar = "[";
        for (int i = 0; i < hungerBarLength; i++)
        {
            hungerBar += i < filledLength ? "=" : " ";
        }
        hungerBar += "]";

        Debug.Log($"Creature Hunger: {hungerBar} {associatedCreature.faim:F1}%");
    }

    void OnTriggerEnter(Collider other)
    {
        // Optional additional collision handling
        FoodItem foodItem = other.GetComponent<FoodItem>();
        if (foodItem != null)
        {
            ConsumeFood();
        }
    }
}