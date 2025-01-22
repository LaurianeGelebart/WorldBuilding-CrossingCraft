using System.Collections.Generic;
using UnityEngine;

public class Population
{
    public float mutationRate = 0.04f;        // Taux de mutation
    public float selectionThreshold = 0.7f;   // Taux de sélection

    private int _populationMaxSize = 50;           // Taille max de la population
    private CreatureGenerator _creatureGenerator; // Référence au générateur de créatures
    private SoundController _soundController; // Référence au controller sonore de créatures
    private List<Creature> _members;
    private GeneticAlgorithm _geneticAlgorithm;

    public List<Creature> Members
    {
        get { return _members; }
    }

    public Population(CreatureGenerator creatureGenerator, SoundController soundController,int populationSize)
    {
        _creatureGenerator = creatureGenerator;
        _soundController = soundController;
        _geneticAlgorithm = new GeneticAlgorithm(mutationRate, selectionThreshold, _creatureGenerator, _soundController);
        InitializePopulation(populationSize);
    }

    public void Update()
    {
        if (_members.Count < _populationMaxSize)
        {
            Evolve(1);
        }

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
            Debug.Log("------------Naissance----------------");
        }
    }

private void InitializePopulation(int populationSize)
{
    _members = new List<Creature>();
    
    int baseCount = populationSize / 2;
    for (int i = 0; i < baseCount; i++)
    {
        Creature newForestCreature = new Creature(_creatureGenerator, _soundController, CreatureType.Forest);
        Creature newDesertCreature = new Creature(_creatureGenerator, _soundController, CreatureType.Desert);
        _members.Add(newForestCreature);
        _members.Add(newDesertCreature);
    }
    
    // Si la population est impair on crée une créature du désert en plus 
    if (populationSize % 2 != 0)
    {
        Creature extraDesertCreature = new Creature(_creatureGenerator, _soundController, CreatureType.Desert);
        _members.Add(extraDesertCreature);
    }
}

    private void CheckIfAlive(Creature creature)
    {
        if (creature.pv < 0)
        {
            Debug.Log("------------Mort----------------");
            _members.Remove(creature);
            creature.Die();
        }
    }


}