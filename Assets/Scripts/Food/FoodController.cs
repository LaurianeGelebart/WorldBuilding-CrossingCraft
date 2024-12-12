using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController : MonoBehaviour
{
    public float hungerDecreaseRate = 1f;     // Vitesse à laquelle la faim diminue
    public float foodSpawnInterval = 10f;     // Intervalle de réapparition de la nourriture
    public GameObject standardFoodPrefab;     // Prefab de nourriture standard à faire spawner
    public GameObject mushroomPrefab;         // Prefab de champignons à faire spawner
    public float standardFoodSpawnRate = 0.8f;  // 80% de nourriture standard
    public float mushroomSpawnRate = 0.2f;      // 20% de champignons

    // Nouveaux paramètres pour le spawn initial
    public int initialFoodSpawnCount = 5;     // Nombre de nourriture à spawner au début
    public float spawnAreaSize = 500f;         // Taille de la zone de spawn

    private Population currentPopulation;
    private float foodSpawnTimer = 0f;
    int maxFoodSwpawn = 8;

    public void SetPopulation(Population population)
    {
        currentPopulation = population;
    }

    void Start()
    {
        // Spawner de la nourriture initiale
        SpawnInitialFood();
    }

    void Update()
    {
        
        // Faire spawner de la nourriture à intervalles réguliers
        foodSpawnTimer += Time.deltaTime;
        if (foodSpawnTimer >= foodSpawnInterval)
        {

            for (int i = 0; i < maxFoodSwpawn; i++)
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
        // Calculer une position aléatoire dans la zone de spawn
        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnAreaSize / 2, spawnAreaSize / 2),
            2f,
            Random.Range(-spawnAreaSize / 2, spawnAreaSize / 2)
        );

        // Décider du type de nourriture à spawner
        float randomValue = Random.value;
        GameObject prefabToSpawn;
        if (randomValue < standardFoodSpawnRate)
        {
            prefabToSpawn = standardFoodPrefab;
        }
        else
        {
            prefabToSpawn = mushroomPrefab;
        }

        if (prefabToSpawn != null)
        {
            // Instancier le prefab à la position de spawn
            GameObject spawnedFood = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

            // Réinitialiser l'échelle de l'objet instancié
            spawnedFood.transform.localScale = new Vector3(20f, 20f, 20f); ;

            // Ajuster la position de l'objet instancié en fonction de son échelle
            spawnedFood.transform.position = new Vector3(
                spawnPosition.x*8 ,
                spawnPosition.y,
                spawnPosition.z*8
            );

            // Ajouter le composant FoodItem si nécessaire
            FoodItem foodItem = spawnedFood.GetComponent<FoodItem>();
            if (foodItem == null)
            {
                foodItem = spawnedFood.AddComponent<FoodItem>();
            }

            // Définir la catégorie de nourriture
            foodItem.foodCategory = (prefabToSpawn == mushroomPrefab)
                ? FoodCategory.Mushroom
                : FoodCategory.Standard;
        }
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