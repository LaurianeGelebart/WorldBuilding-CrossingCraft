using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CreatureGenerator creatureGenerator; // Référence au générateur de créatures


    private Population creatures;

    public FoodController foodController;  // Référence au FoodController


    void Start()
    {
        creatures = new Population(creatureGenerator);
        foodController.SetPopulation(creatures);  // Passer la référence de Population
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
        foreach (var creature in creatures.Members)
        {
            foodController.UpdateCreatureHunger(creature);
        }
    }
}
