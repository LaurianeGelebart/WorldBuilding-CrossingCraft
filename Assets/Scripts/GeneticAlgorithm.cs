using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm<Creature>
{
    private int populationSize;         // Taille de la actualGeneration
    private float mutationRate;      // Taux de mutation
    private float selectionThreshold; // Taux de sélection
    private float fitnessImportanceBias = 0.5f; // Biais de sélection
    

    public GeneticAlgorithm(int populationSize, float mutationRate, float selectionThreshold)
    {
        this.populationSize = populationSize;
        this.mutationRate = mutationRate;
        this.selectionThreshold = selectionThreshold;
    }
    

    // Exécute l'évolution de la actualGeneration sur un nombre de générations défini
    public List<Creature> EvolvePopulation(int generations, List<Creature> actualGeneration)
    {
        int iteration = 0; 
        while (iteration < generations)
        {
            // Sélectionner les meilleures créatures
            List<Creature> selectedPopulation = Selection(actualGeneration);

            // Créer la nouvelle génération
            List<Creature> newPopulation = new List<Creature>();
            while (newPopulation.Count < populationSize)
            {
                Creature parent1 = selectedPopulation[Random.Range(0, selectedPopulation.Count)];
                Creature parent2 = selectedPopulation[Random.Range(0, selectedPopulation.Count)];

                Creature child = Recombination(parent1, parent2); 
                Mutate(child); 
                newPopulation.Add(child);
            }

            // Remplacer l'ancienne actualGeneration par la nouvelle
            actualGeneration = newPopulation;

            iteration++; 
        }
        return actualGeneration;
    }

    // Sélectionne les créatures les plus aptes pour la reproduction
    private List<Creature> Selection(List<Creature> actualGeneration)
    {
        List<Creature> selectedPopulation = new List<Creature>();

        // Trier la actualGeneration par fitness décroissante
        actualGeneration.Sort((a, b) => b.fitness.CompareTo(a.fitness));

        foreach (var creature in actualGeneration)
        {
            float selectionChance = creature.fitness + Random.Range(0f, 1f) * creature.fitness * 0.1f;
            if (selectionChance >= (actualGeneration[0].fitness * fitnessImportanceBias)) 
            {
                selectedPopulation.Add(creature);
            }

            if (selectedPopulation.Count >= (int)(populationSize * selectionThreshold))
            {
                break;
            }
        }

        return selectedPopulation;
    }

    // Crée une nouvelle créature à partir de la recombinaison de deux parents
    private Creature Recombination(Creature parent1, Creature parent2)
    {
        List<int> genome = new List<int>(new int[parent1.genomeLength]);
        int crossoverPoint = Random.Range(0, parent1.genomeLength / 2);

        for (int i = 0; i < parent1.genomeLength; i++)
        {
            genome[i] = (i < crossoverPoint) ? parent1.genome[i] : parent2.genome[i];
        }

        return new Creature(genome, creatureGenerator);
    }

    // Applique une mutation sur le génome d'une créature selon le taux de mutation
    private void Mutate(Creature creature)
    {
        for (int i = 0; i < creature.genome.Count; i++)
        {
            if (Random.Range(0f, 1f) < mutationRate)
            {
                creature.genome[i] = 1 - creature.genome[i];  // Inverser le bit (0 -> 1, 1 -> 0)
            }
        }
    }

}
