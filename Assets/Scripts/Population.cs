using System.Collections.Generic;
using UnityEngine;

public class Population 
{
    public float mutationRate = 0.01f;        // Taux de mutation
    public float selectionThreshold = 0.7f;   // Taux de sélection
     
    private int _populationSize;           // Taille de la population
    private CreatureGenerator _creatureGenerator; // Référence au générateur de créatures
    private List<Creature> _members;
    private GeneticAlgorithm _geneticAlgorithm;
     
    public List<Creature> Members 
    {
        get { return _members; }
    }
     
    public Population(CreatureGenerator creatureGenerator, int populationSize)
    {
        _creatureGenerator = creatureGenerator;
        _populationSize = populationSize;
         
        _geneticAlgorithm = new GeneticAlgorithm(populationSize, mutationRate, selectionThreshold, _creatureGenerator);
        InitializePopulation();
    }
     
    public void Update()
    {
        Evolve(1);
        
        for (int i = _members.Count - 1; i >= 0; i--) // On itère à l'envers car on supprime des membres de la liste
        {
            _members[i].UpdatePv();
            CheckIfAlive(_members[i]);
        }
    }
     
    public void Evolve(int generationNumber)
    {
        List<Creature> newGeneration = _geneticAlgorithm.EvolvePopulation(generationNumber, _members);
        foreach (Creature newCreature in newGeneration)
        {
            _members.Add(newCreature);
        }
    }
     
    private void InitializePopulation()
    {
        _members = new List<Creature>();
         
        for (int i = 0; i < _populationSize; i++)
        {
            Creature newCreature = new Creature(_creatureGenerator);
            _members.Add(newCreature);
        }
    }
    
    private void CheckIfAlive(Creature creature)
    {
        if(creature.pv < 0) 
        {
            Debug.Log("------------Mort----------------");
            _members.Remove(creature);
            creature.Die();
        }
    }
     
    
}