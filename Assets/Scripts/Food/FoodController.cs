using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Configuration de spawn pour un type de nourriture dans l'environnement de jeu
/// </summary>
[System.Serializable]
public class FoodSpawnConfig
{
    public FoodCategory category;      // Catégorie de nourriture
    public GameObject prefab;          // Prefab à instancier
    public float spawnChance = 0.2f;   // Probabilité de spawn
    public float nutritionalValue = 50f;  // Valeur nutritionnelle
    public float poisonIntensity = 0f;    // Intensité de poison
    public FoodEnvironment environment; // Environnement de spawn
}

/// <summary>
/// Contrôle la génération et la gestion de la nourriture dans l'environnement de simulation
/// </summary>
public class FoodController : MonoBehaviour
{
    public List<FoodSpawnConfig> foodTypes = new List<FoodSpawnConfig>();

    public float foodSpawnInterval = 10f;  // Intervalle entre chaque spawn de nourriture
    public int maxFoodSpawn = 80;          // Nombre maximum de nourriture générée
    public float spawnAreaSize = 600f;     // Taille de la zone de spawn
    public float hungerDecreaseRate = 1f;  // Taux de diminution de la faim
    public int initialFoodSpawnCount = 5;  // Nombre initial de nourriture

    public Vector3 min, max;               // Limites de la zone de spawn

    public TerrainController terrainController;  // Référence au contrôleur de terrain

    private Population currentPopulation;  // Population actuelle de créatures
    private float foodSpawnTimer = 0f;     // Minuteur pour le spawn de nourriture

    /// <summary>
    /// Définit la population active dans la simulation
    /// </summary>
    /// <param name="population">Population de créatures à gérer</param>
    public void SetPopulation(Population population)
    {
        currentPopulation = population;
    }


    void Update()
    {
        // Gère le spawn périodique de nourriture
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

    /// <summary>
    /// Génère une quantité initiale de nourriture au début de la simulation
    /// </summary>
    public void SpawnInitialFood()
    {
        for (int i = 0; i < initialFoodSpawnCount; i++)
        {
            SpawnFood();
        }
    }

    /// <summary>
    /// Génère un élément de nourriture à une position aléatoire dans l'environnement
    /// </summary>
    void SpawnFood()
    {
        if (foodTypes.Count == 0) return;

        // Générer une position de spawn aléatoire
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

        // Filtrer les types de nourriture selon l'environnement
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

        // Sélectionner un type de nourriture aléatoirement parmi les types filtrés
        FoodSpawnConfig selectedFood = filteredFoodTypes
            [Random.Range(0, filteredFoodTypes.Count)];

        // Instancier la nourriture
        GameObject spawnedFood = Instantiate(selectedFood.prefab);

        // Configurer les propriétés de l'objet de nourriture
        FoodItem foodItem = spawnedFood.GetComponent<FoodItem>() ?? spawnedFood.AddComponent<FoodItem>();
        foodItem.foodCategory = selectedFood.category;
        foodItem.nutritionalValue = selectedFood.nutritionalValue;
        foodItem.poisonIntensity = selectedFood.poisonIntensity;

        // Ajuster l'échelle et la position
        spawnedFood.transform.localScale = new Vector3(20f, 20f, 20f);
        spawnedFood.transform.position = new Vector3(
            spawnPosition.x,
            spawnPosition.y,
            spawnPosition.z
        );
    }

    /// <summary>
    /// Met à jour le niveau de faim d'une créature
    /// </summary>
    /// <param name="creature">Créature dont le niveau de faim doit être mis à jour</param>
    public void UpdateCreatureHunger(Creature creature)
    {
        // Diminuer la faim de la créature au fil du temps
        creature.faim -= hungerDecreaseRate * Time.deltaTime;

        // Gérer la mort si la faim atteint zéro
        if (creature.faim <= 0)
        {
            creature.faim = 0;
            creature.Die(); // Appeler la méthode Die() quand la faim est à 0
        }
    }

    /// <summary>
    /// Retrouve une créature à partir de son collider
    /// </summary>
    /// <param name="collider">Collider de la créature recherchée</param>
    /// <returns>La créature correspondante ou null si non trouvée</returns>
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

/// <summary>
/// Énumération des environnements possibles pour la nourriture
/// </summary>
public enum FoodEnvironment
{
    Forest,  // Environnement forestier
    Desert,  // Environnement désertique
    Both     // Applicable dans tous les environnements
}