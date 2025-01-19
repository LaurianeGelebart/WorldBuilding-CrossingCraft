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
    public FoodEnvironment environment;
}

public class FoodController : MonoBehaviour
{
    public List<FoodSpawnConfig> foodTypes = new List<FoodSpawnConfig>();

    public float foodSpawnInterval = 10f;
    public int maxFoodSpawn = 80;
    public float spawnAreaSize = 600f;
    public float hungerDecreaseRate = 1f;
    public int initialFoodSpawnCount = 5;

    public Vector3 min, max;

    public TerrainController terrainController;

    private Population currentPopulation;
    private float foodSpawnTimer = 0f;

    public void SetPopulation(Population population)
    {
        currentPopulation = population;
    }

    void Start()
    {
        // Spawn initial food
        // SpawnInitialFood();
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

    public void SpawnInitialFood()
    {
        for (int i = 0; i < initialFoodSpawnCount; i++)
        {
            SpawnFood();
        }
    }

    void SpawnFood()
    {
        if (foodTypes.Count == 0) return;

        // Position de spawn aléatoire
        Vector3 spawnPosition = new(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            Random.Range(min.z, max.z)
        );
        Vector3Int gridPos;
        WFCTile tile = null;
        bool found = false;
        while (!found)
        {
            if (spawnPosition.y < terrainController.min.y)
            {
                Debug.Log($"No tile found at {spawnPosition}");
                spawnPosition = new(
                    Random.Range(min.x, max.x),
                    Random.Range(min.y, max.y),
                    Random.Range(min.z, max.z)
                );
                tile = null;
            }
            gridPos = terrainController.WorldToGrid(spawnPosition);
            try
            {
                tile = terrainController.wfc.GetTileAt(gridPos);
            }
            catch
            {
                spawnPosition.y -= 1;
                continue;
            }
            if (tile.prefab != null)
            {
                found = true;
            }
            spawnPosition.y -= 1;

        }

        // Sélection aléatoire du type de nourriture
        var filteredFoodTypes = foodTypes
            .FindAll(food =>
                tile == null ||
                food.environment == FoodEnvironment.Both ||
                TileName.IsTransition(tile.Name) ||
                (
                    food.environment == FoodEnvironment.Forest &&
                    TileName.IsForest(tile.Name)
                ) || (
                    food.environment == FoodEnvironment.Desert &&
                    TileName.IsDesert(tile.Name)
                ));

        FoodSpawnConfig selectedFood = filteredFoodTypes
            [Random.Range(0, filteredFoodTypes.Count)];

        // Instancier la nourriture
        GameObject spawnedFood = Instantiate(selectedFood.prefab);

        // Configurer l'item de nourriture
        FoodItem foodItem = spawnedFood.GetComponent<FoodItem>() ?? spawnedFood.AddComponent<FoodItem>();
        foodItem.foodCategory = selectedFood.category;
        foodItem.nutritionalValue = selectedFood.nutritionalValue;
        foodItem.poisonIntensity = selectedFood.poisonIntensity;

        // Ajuster l'échelle
        spawnedFood.transform.localScale = new Vector3(20f, 20f, 20f);

        // Ajuster la position
        spawnedFood.transform.position = new Vector3(
            spawnPosition.x,
            spawnPosition.y,
            spawnPosition.z
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

public enum FoodEnvironment
{
    Forest,
    Desert,
    Both,
}