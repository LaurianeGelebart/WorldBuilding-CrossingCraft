using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class Population<C> where C : Creature
public class Population
{
    public int populationSize = 10;           // Taille de la population
    public float mutationRate = 0.01f;        // Taux de mutation
    public float selectionThreshold = 0.7f;   // Taux de sélection
  
    public CreatureGenerator creatureGenerator; // Référence au générateur de créatures
    private List<Creature> members;
    private GeneticAlgorithm geneticAlgorithm;

    public List<Creature> Members
    {
        get { return members; }  
    }

    public Population(CreatureGenerator creatureGenerator)
    {
        this.creatureGenerator = creatureGenerator;

        this.geneticAlgorithm = new GeneticAlgorithm(populationSize, mutationRate, selectionThreshold, creatureGenerator);
        this.initializePopulation();
    }

    public void evolve() 
    {
        this.geneticAlgorithm.EvolvePopulation(1, members);
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
