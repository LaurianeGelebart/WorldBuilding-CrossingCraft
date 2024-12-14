using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CreatureGenerator creatureGenerator; // Référence au générateur de créatures
   
    public SoundController soundController; // Référence au controller sonore
    
    public FoodController foodController;  // Référence au FoodController

    public int beginningCreatureNumber = 30; // Nombre de créatures dans la population de départ 

    private Population _creaturesPopulation;


    void Start()
    {
        _creaturesPopulation = new Population(creatureGenerator, soundController, beginningCreatureNumber);
        _creaturesPopulation.Evolve(1);
        foodController.SetPopulation(_creaturesPopulation);
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
