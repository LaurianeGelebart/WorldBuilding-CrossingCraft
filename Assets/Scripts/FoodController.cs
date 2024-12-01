using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController : MonoBehaviour
{
    public float hungerDecreaseRate = 1f;     // Vitesse à laquelle la faim diminue
    public float foodValue = 50f;             // Quantité de nourriture qui restaure la faim
    public float foodSpawnInterval = 10f;     // Intervalle de réapparition de la nourriture
    public GameObject foodPrefab;             // Prefab de nourriture à faire spawn
    private Population currentPopulation;

    private float foodSpawnTimer = 0f;

    public void SetPopulation(Population population)
    {
        currentPopulation = population;
    }

    void Update()
    {
        // Faire spawner de la nourriture à intervalles réguliers
        foodSpawnTimer += Time.deltaTime;
        if (foodSpawnTimer >= foodSpawnInterval)
        {
            SpawnFood();
            foodSpawnTimer = 0f;
        }
    }

    void SpawnFood()
    {
        // Spawner la nourriture à une position aléatoire dans la scène
        Vector3 spawnPosition = new Vector3(
            Random.Range(0, 50),
            0.5f,
            Random.Range(0, 50)
        );

        if (foodPrefab != null)
        {
            Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
        }
    }

    // À ajouter dans le GameController ou à appeler depuis celui-ci
    public void UpdateCreatureHunger(Creature creature)
    {
        // Diminuer la faim de la créature au fil du temps
        creature.faim -= hungerDecreaseRate * Time.deltaTime;

        // Si la faim tombe en dessous de zéro, on peut ajouter des conséquences
        if (creature.faim <= 0)
        {
            creature.faim = 0;
            // Potentiellement réduire la fitness ou faire mourir la créature
        }
    }

    /*void OnTriggerEnter(Collider other)
    {
        // Vérifier si l'objet qui entre en collision est de la nourriture
        if (other.CompareTag("Food"))
        {
            // Trouver la créature la plus proche qui peut manger
            Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);
            foreach (Collider collider in colliders)
            {
                Creature creature = GetCreatureFromCollider(collider);
                if (creature != null)
                {
                    // Augmenter la faim de la créature
                    creature.faim += foodValue;

                    // Détruire la nourriture
                    Destroy(other.gameObject);
                    break;
                }
            }
        }
    }*/

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
