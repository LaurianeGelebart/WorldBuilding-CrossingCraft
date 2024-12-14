using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GeneticAlgorithm
{
    private float _mutationRate;      // Taux de mutation
    private float _selectionThreshold; // Taux de sélection
    private float _fitnessImportanceBias = 0.5f; // Biais de sélection

    private CreatureGenerator _creatureGenerator;
    private SoundController _soundController;


    public GeneticAlgorithm(float mutationRate, float selectionThreshold, CreatureGenerator creatureGenerator, SoundController soundController)
    {
        _mutationRate = mutationRate;
        _selectionThreshold = selectionThreshold;
        _creatureGenerator = creatureGenerator;
        _soundController = soundController;
    }



    public List<Creature> EvolvePopulation(int generations, List<Creature> actualGeneration)
    {
        int iteration = 0;

        while (iteration < generations)
        {
            // Create a new list to store the population for this iteration
            List<Creature> newGenerationPopulation = new List<Creature>();

            // Separate selection for Forest and Desert populations
            List<Creature> selectedForestPopulation = Selection(actualGeneration, CreatureType.Forest);
            List<Creature> selectedDesertPopulation = Selection(actualGeneration, CreatureType.Desert);

            // Evolve each population separately
            List<Creature> newForestPopulation = EvolveSpecificPopulation(selectedForestPopulation);
            List<Creature> newDesertPopulation = EvolveSpecificPopulation(selectedDesertPopulation);

            // Combine the new populations
            newGenerationPopulation.AddRange(newForestPopulation);
            newGenerationPopulation.AddRange(newDesertPopulation);

            // Update actual generation for next iteration
            actualGeneration = newGenerationPopulation;

            iteration++;
        }

        return actualGeneration;
    }


    // Sélectionne les créatures les plus aptes pour la reproduction
    private List<Creature> Selection(List<Creature> actualGeneration, CreatureType creatureType)
    {
        List<Creature> selectedPopulation = new List<Creature>();

        // Filter the population to keep only creatures of the specified type
        List<Creature> filteredPopulation = actualGeneration.Where(c => c.Type == creatureType).ToList();

        // Trier la filteredPopulation par fitness décroissante
        filteredPopulation.Sort((a, b) => b.fitness.CompareTo(a.fitness));

        foreach (var creature in filteredPopulation)
        {
            float selectionChance = creature.fitness + Random.Range(0f, 1f) * creature.fitness * 0.1f;
            if (selectionChance >= (filteredPopulation[0].fitness * _fitnessImportanceBias))
            {
                selectedPopulation.Add(creature);
            }

            if (selectedPopulation.Count >= (int)(filteredPopulation.Count * _selectionThreshold))
            {
                break;
            }
        }

        return selectedPopulation;
    }


    private List<Creature> EvolveSpecificPopulation(List<Creature> selectedPopulation)
    {
        List<Creature> newPopulation = new List<Creature>();

        while (newPopulation.Count < selectedPopulation.Count / 2)
        {
            // Select parents randomly from the selected population
            Creature parent1 = selectedPopulation[Random.Range(0, selectedPopulation.Count)];
            Creature parent2 = selectedPopulation[Random.Range(0, selectedPopulation.Count)];

            // Create child through recombination
            Creature child = Recombination(parent1, parent2);

            // Mutate the child
            Mutate(child);

            newPopulation.Add(child);
        }

        return newPopulation;
    }



    // Crée une nouvelle créature à partir de la recombinaison de deux parents s'ils sont du même type, sinon retourne null 
    private Creature Recombination(Creature parent1, Creature parent2)
    {
        // Vérifier que les parents sont du même type
        if (parent1.Type != parent2.Type)
        {
            return null;
        }

        List<int> genome = new List<int>(new int[parent1.genomeLength]);
        int crossoverPoint = Random.Range(1, (parent1.genomeLength / 2) + 1); // crossover tous les différents gènes en évitant le premier 

        for (int i = 0; i < parent1.genomeLength; i++)
        {
            genome[i] = (i < crossoverPoint) ? parent1.genome[i] : parent2.genome[i];
        }

        return new Creature(genome, _creatureGenerator, _soundController);
    }

    // Applique une mutation sur le génome d'une créature selon le taux de mutation
    private void Mutate(Creature creature)
    {
        for (int i = 0; i < creature.genome.Count; i++)
        {
            if (Random.Range(0f, 1f) < _mutationRate)
            {
                creature.genome[i] = 1 - creature.genome[i];  // Inverser le bit (0 -> 1, 1 -> 0)
            }
        }
    }

}
