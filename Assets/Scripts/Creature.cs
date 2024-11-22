using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Creature
{
    public List<int> genome;           // Liste d'entier (0 ou 1) représentant le génome de la créature (suite de bits)
    public float fitness;              // Fitness de la créature (évaluation de sa qualité)
    public GameObject model;           // Modèle 3D associé à la créature
    public int genomeLength = 10;           // Longueur du génome

    protected CreatureGenerator<Creature> creatureGenerator;
    public abstract void DecodeGenome();
    public abstract void EvaluateFitness();
    public abstract void Update();

    /// <summary>
    /// Constructeur pour créer une créature avec un génome aléatoire d'une longueur donnée
    /// Décode le génome et génère un modèle 3D
    /// </summary>
    /// <param name="generator">Référence au générateur de modèles</param>
    public Creature(CreatureGenerator<Creature> generator)
    {
        genome = new List<int>();
        creatureGenerator = generator;

        // Remplissage du génome avec des valeurs aléatoires (0 ou 1)
        for (int i = 0; i < genomeLength; i++)         
        {
            genome.Add(Random.Range(0, 2)); 
        }

        DecodeGenome();
        EvaluateFitness();
        model = creatureGenerator.GenerateModel(this); 
    } 

    /// <summary>
    /// Constructeur pour créer une créature à partir d'un génome déjà existant
    /// Décode le génome et génère un modèle 3D
    /// </summary>
    /// <param name="generatedGenome">Génome prédéfini</param>
    /// <param name="generator">Référence au générateur de modèles</param>
    public Creature(List<int> generatedGenome, CreatureGenerator<Creature> generator)
    {
        genome = generatedGenome;                      
        creatureGenerator = generator;

        DecodeGenome();       
        EvaluateFitness();       
        model = creatureGenerator.GenerateModel(this); 
    }


}
