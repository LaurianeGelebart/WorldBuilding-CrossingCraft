using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CreatureGenerator creatureGenerator; // Référence au générateur de créatures


    private Population creaturesPopulation;

    public FoodController foodController;  // Référence au FoodController


    void Start()
    {
        creaturesPopulation = new Population(creatureGenerator);
        creaturesPopulation.evolve();
        foodController.SetPopulation(creaturesPopulation);  // Passer la référence de Population
    }

    void Update()
    {
        // foreach(var creature in creatures.Population){
        //     creature.Update();
        // }
        // foreach(var creature in desertCreatures.Population){
        //     creature.Update();
        // }

        FoodController foodController = FindObjectOfType<FoodController>();
        foreach (var creature in creaturesPopulation.Members)
        {
            foodController.UpdateCreatureHunger(creaturesPopulation);
        }
    }
}
