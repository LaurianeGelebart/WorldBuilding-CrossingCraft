using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CreatureGenerator creatureGenerator; // Référence au générateur de créatures


    private Population creatures;


    void Start()
    {
        creatures = new Population(creatureGenerator);
    }

    void Update()
    {
        // foreach(var creature in creatures.Population){
        //     creature.Update();
        // }
        // foreach(var creature in desertCreatures.Population){
        //     creature.Update();
        // }
    }
}
