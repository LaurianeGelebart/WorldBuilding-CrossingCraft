using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CreatureGenerator creatureGenerator; // Référence au générateur de créatures

    private Population _creaturesPopulation;

    public FoodController foodController;  // Référence au FoodController


    void Start()
    {
        _creaturesPopulation = new Population(creatureGenerator, 10);
        _creaturesPopulation.Evolve(1);
        foodController.SetPopulation(_creaturesPopulation);  // Passer la référence de Population
    }

    void Update()
    {
        _creaturesPopulation.Update();
        FoodController foodController = FindObjectOfType<FoodController>();
        foreach (var creature in _creaturesPopulation.Members)
        {
            foodController.UpdateCreatureHunger(creature);
        }
    }
}
