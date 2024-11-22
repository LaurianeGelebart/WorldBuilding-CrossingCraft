using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population<C> where C : Creature
{
    public int populationSize = 10;           // Taille de la population
    // public int generations = 10;              // Nombre de générations
    public float mutationRate = 0.01f;        // Taux de mutation
    public float selectionThreshold = 0.7f;   // Taux de sélection
  
    public CreatureGenerator<C> creatureGenerator; // Référence au générateur de créatures
    private List<C> members;
    private GeneticAlgorithm<C> geneticAlgorithm;

    public List<C> Members
    {
        get { return members; }  
    }

    public Population(CreatureGenerator<C> creatureGenerator)
    {
        this.creatureGenerator = creatureGenerator;

        this.geneticAlgorithm = new GeneticAlgorithm<C>(populationSize, mutationRate, selectionThreshold);
        this.initializePopulation();
    }

    public void evolve() 
    {
        this.geneticAlgorithm.EvolvePopulation(this);
    }

    public void initializePopulation()
    {
        members = new List<Creature>();

        for (int i = 0; i < populationSize; i++)
        {
            Creature newCreature = new Creature(creatureGenerator);
            members.Add(newCreature);
        }
    }

    // public Creature newCreature(List<int> generatedGenome){
    //     return new Creature(generatedGenome, this.creatureGenerator);
    // }


}
