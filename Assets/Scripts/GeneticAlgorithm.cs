using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm
// public class GeneticAlgorithm<C> where C : ForestCreature
{
    private int populationSize;         // Taille de la actualGeneration
    private float mutationRate;      // Taux de mutation
    private float selectionThreshold; // Taux de sélection
    private float fitnessImportanceBias = 0.5f; // Biais de sélection

    private ForestCreatureGenerator creatureGenerator;
    

    public GeneticAlgorithm(int populationSize, float mutationRate, float selectionThreshold, ForestCreatureGenerator creatureGenerator)
    {
        this.populationSize = populationSize;
        this.mutationRate = mutationRate;
        this.selectionThreshold = selectionThreshold;
        this.creatureGenerator = creatureGenerator;
    }
    

    // Exécute l'évolution de la actualGeneration sur un nombre de générations défini
    public List<ForestCreature> EvolvePopulation(int generations, List<ForestCreature> actualGeneration)
    {
        int iteration = 0; 
        while (iteration < generations)
        {
            // Sélectionner les meilleures créatures
            List<ForestCreature> selectedPopulation = Selection(actualGeneration);

            // Créer la nouvelle génération
            List<ForestCreature> newPopulation = new List<ForestCreature>();
            while (newPopulation.Count < populationSize)
            {
                ForestCreature parent1 = selectedPopulation[Random.Range(0, selectedPopulation.Count)];
                ForestCreature parent2 = selectedPopulation[Random.Range(0, selectedPopulation.Count)];

                ForestCreature child = Recombination(parent1, parent2); 
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
    private List<ForestCreature> Selection(List<ForestCreature> actualGeneration)
    {
        List<ForestCreature> selectedPopulation = new List<ForestCreature>();

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
    private ForestCreature Recombination(ForestCreature parent1, ForestCreature parent2)
    {
        List<int> genome = new List<int>(new int[parent1.genomeLength]);
        int crossoverPoint = Random.Range(0, parent1.genomeLength / 2);

        for (int i = 0; i < parent1.genomeLength; i++)
        {
            genome[i] = (i < crossoverPoint) ? parent1.genome[i] : parent2.genome[i];
        }

        return new ForestCreature(genome, creatureGenerator);
    }

    // Applique une mutation sur le génome d'une créature selon le taux de mutation
    private void Mutate(ForestCreature creature)
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
