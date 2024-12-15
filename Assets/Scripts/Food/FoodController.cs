using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FoodSpawnConfig
{
    public FoodCategory category;
    public GameObject prefab;
    public float spawnChance = 0.2f;
    public float nutritionalValue = 50f;
    public float poisonIntensity = 0f;
}

public class FoodController : MonoBehaviour
{
    public List<FoodSpawnConfig> foodTypes = new List<FoodSpawnConfig>();

    public float foodSpawnInterval = 10f;
    public int maxFoodSpawn = 80;
    public float spawnAreaSize = 600f;
    public float hungerDecreaseRate = 1f;
    public int initialFoodSpawnCount = 5;

    private Population currentPopulation;
    private float foodSpawnTimer = 0f;

    public void SetPopulation(Population population)
    {
        currentPopulation = population;
    }

    void Start()
    {
        // Spawn initial food
        SpawnInitialFood();
    }

    void Update()
    {
        // Spawn food at intervals
        foodSpawnTimer += Time.deltaTime;
        if (foodSpawnTimer >= foodSpawnInterval)
        {
            for (int i = 0; i < maxFoodSpawn; i++)
            {
                SpawnFood();
            }
            foodSpawnTimer = 0f;
        }
    }

    void SpawnInitialFood()
    {
        for (int i = 0; i < initialFoodSpawnCount; i++)
        {
            SpawnFood();
        }
    }

    void SpawnFood()
    {
        if (foodTypes.Count == 0) return;

        // Sélection aléatoire du type de nourriture
        FoodSpawnConfig selectedFood = foodTypes[Random.Range(0, foodTypes.Count)];

        // Position de spawn aléatoire
        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnAreaSize * 2, spawnAreaSize / 2),
            2f,
            Random.Range(-spawnAreaSize / 2, spawnAreaSize / 2)
        );

        // Instancier la nourriture
        GameObject spawnedFood = Instantiate(selectedFood.prefab, spawnPosition, Quaternion.identity);

        // Configurer l'item de nourriture
        FoodItem foodItem = spawnedFood.GetComponent<FoodItem>() ?? spawnedFood.AddComponent<FoodItem>();
        foodItem.foodCategory = selectedFood.category;
        foodItem.nutritionalValue = selectedFood.nutritionalValue;
        foodItem.poisonIntensity = selectedFood.poisonIntensity;

        // Ajuster l'échelle
        spawnedFood.transform.localScale = new Vector3(20f, 20f, 20f);

        // Ajuster la position
        spawnedFood.transform.position = new Vector3(
            spawnPosition.x * 8,
            spawnPosition.y,
            spawnPosition.z * 8
        );
    }

    public void UpdateCreatureHunger(Creature creature)
    {
        // Diminuer la faim de la créature au fil du temps
        creature.faim -= hungerDecreaseRate * Time.deltaTime;

        // Si la faim tombe en dessous de zéro, on peut ajouter des conséquences
        if (creature.faim <= 0)
        {
            creature.faim = 0;
            creature.Die(); // Appeler la méthode Die() quand la faim est à 0
        }
    }

    Creature GetCreatureFromCollider(Collider collider)
    {
        if (currentPopulation == null) return null;
        foreach (Creature creature in currentPopulation.Members)
        {
            if (creature.creatureCollider == collider)
            {
                return creature;
            }
        }
        return null;
    }
}