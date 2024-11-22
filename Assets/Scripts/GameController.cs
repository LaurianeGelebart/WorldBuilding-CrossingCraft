using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public ForestCreatureGenerator forestCreatureGenerator; // Référence au générateur de créatures
    public DesertCreatureGenerator<DesertCreature> desertCreatureGenerator; // Référence au générateur de créatures


    private Population<ForestCreature> forestCreatures;
    private Population<DesertCreature> desertCreatures;


    void Start()
    {
        forestCreatures = new Population<ForestCreature>( forestCreatureGenerator);
        desertCreatures = new Population<DesertCreature>(desertCreatureGenerator);
    }

    void Update()
    {
        // foreach(var creature in forestCreatures.Population){
        //     creature.Update();
        // }
        // foreach(var creature in desertCreatures.Population){
        //     creature.Update();
        // }
    }
}
