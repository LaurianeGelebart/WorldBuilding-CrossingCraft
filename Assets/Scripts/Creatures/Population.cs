using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Population de créatures (contrôle son évolution dans le temps)
/// </summary>
public class Population 
{
    private float _mutationRate;         // Taux de mutation génétique
    private float _selectionThreshold;   // Seuil de sélection naturelle
    private int _populationMaxSize = 50; // Taille maximale de la population

    private CreatureGenerator _creatureGenerator; // Générateur de créatures
    private SoundController _soundController;    // Contrôleur sonore
    private List<Creature> _members;             // Liste des créatures
    private GeneticAlgorithm _geneticAlgorithm;  // Algorithme génétique

    /// <summary>
    /// Propriété d'accès à la liste des membres de la population
    /// </summary>
    public List<Creature> Members 
    {
        get { return _members; }
    }

    /// <summary>
    /// Constructeur de la population
    /// </summary>
    /// <param name="creatureGenerator">Générateur utilisé pour créer de nouvelles créatures</param>
    /// <param name="soundController">Contrôleur sonore pour les événements de créatures</param>
    /// <param name="populationSize">Nombre initial de créatures</param>
    /// <param name="mutationRate">Taux de mutation génétique</param>
    /// <param name="selectionThreshold">Seuil de sélection naturelle</param>
    public Population(
        CreatureGenerator creatureGenerator, 
        SoundController soundController,
        int populationSize, 
        float mutationRate, 
        float selectionThreshold)
    {
        _creatureGenerator = creatureGenerator;
        _soundController = soundController;
        _mutationRate = mutationRate;
        _selectionThreshold = selectionThreshold;
        _geneticAlgorithm = new GeneticAlgorithm(
            _mutationRate, 
            _selectionThreshold, 
            _creatureGenerator, 
            _soundController
        );
        InitializePopulation(populationSize);
    }

    /// <summary>
    /// Mise à jour périodique de la population
    /// </summary>
    public void Update()
    {
        // Faire évoluer la population si elle n'a pas atteint sa taille maximale
        if (_members.Count < _populationMaxSize)
        {
            Evolve(1);
        }

        // Mettre à jour et vérifier l'état de chaque créature
        for (int i = _members.Count - 1; i >= 0; i--)
        {
            _members[i].UpdatePv();
            CheckIfAlive(_members[i]);
        }
    }

    /// <summary>
    /// Fait évoluer la population sur un nombre donné de générations
    /// </summary>
    /// <param name="generationNumber">Nombre de générations à faire évoluer</param>
    public void Evolve(int generationNumber)
    {
        // Générer une nouvelle génération via l'algorithme génétique
        List<Creature> newGeneration = _geneticAlgorithm.EvolvePopulation(
            generationNumber, _members
        );

        // Ajouter les nouvelles créatures à la population
        foreach (Creature newCreature in newGeneration)
        {
            _members.Add(newCreature);
        }
    }

    /// <summary>
    /// Initialise la population avec un mélange de créatures forestières et désertiques
    /// </summary>
    /// <param name="populationSize">Taille totale de la population à initialiser</param>
    private void InitializePopulation(int populationSize) 
    {
        _members = new List<Creature>();

        int baseCount = populationSize / 2;
        for (int i = 0; i < baseCount; i++)
        {
            // Créer des créatures forestières et désertiques
            Creature newForestCreature = new Creature(
                _creatureGenerator, _soundController, CreatureType.Forest
            );
            Creature newDesertCreature = new Creature(
                _creatureGenerator, _soundController, CreatureType.Desert
            );
            _members.Add(newForestCreature);
            _members.Add(newDesertCreature);
        }

        // Ajouter une créature supplémentaire si la population est impaire
        if (populationSize % 2 != 0)
        {
            Creature extraDesertCreature = new Creature(
                _creatureGenerator, _soundController, CreatureType.Desert
            );
            _members.Add(extraDesertCreature);
        }
    }

    /// <summary>
    /// Vérifie et gère la mort d'une créature
    /// </summary>
    /// <param name="creature">Créature à vérifier</param>
    private void CheckIfAlive(Creature creature)
    {
        // Retirer la créature si ses points de vie sont négatifs
        if (creature.pv < 0)
        {
            _members.Remove(creature);
            creature.Die();
        }
    }
}