using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class Population<C> where C : ForestCreature
public class Population
{
    public int populationSize = 10;           // Taille de la population
    // public int generations = 10;              // Nombre de générations
    public float mutationRate = 0.01f;        // Taux de mutation
    public float selectionThreshold = 0.7f;   // Taux de sélection
  
    public ForestCreatureGenerator creatureGenerator; // Référence au générateur de créatures
    private List<ForestCreature> members;
    private GeneticAlgorithm geneticAlgorithm;

    public List<ForestCreature> Members
    {
        get { return members; }  
    }

    public Population(ForestCreatureGenerator creatureGenerator)
    {
        this.creatureGenerator = creatureGenerator;

        this.geneticAlgorithm = new GeneticAlgorithm(populationSize, mutationRate, selectionThreshold, creatureGenerator);
        this.initializePopulation();
    }

    public void evolve() 
    {
        this.geneticAlgorithm.EvolvePopulation(2, members);
    }

    public void initializePopulation()
    {
        members = new List<ForestCreature>();

        for (int i = 0; i < populationSize; i++)
        {
            ForestCreature newCreature = new ForestCreature(populationSize, creatureGenerator);
            members.Add(newCreature);
        }
    }

    // public Creature newCreature(List<int> generatedGenome){
    //     return new Creature(generatedGenome, this.creatureGenerator);
    // }


}
