using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GeneticAlgorithm
{
    private float _mutationRate;      // Taux de mutation des gènes
    private float _selectionThreshold; // Seuil de sélection des créatures
    private float _fitnessImportanceBias = 0.5f; // Pondération de la fitness

    private CreatureGenerator _creatureGenerator;
    private SoundController _soundController;

    /// <summary>
    /// Constructeur de l'algorithme génétique
    /// </summary>
    /// <param name="mutationRate">Probabilité de mutation des gènes</param>
    /// <param name="selectionThreshold">Proportion de créatures sélectionnées pour reproduction</param>
    /// <param name="creatureGenerator">Générateur de modèles de créatures</param>
    /// <param name="soundController">Contrôleur audio</param>
    public GeneticAlgorithm(float mutationRate, float selectionThreshold, CreatureGenerator creatureGenerator, SoundController soundController)
    {
        _mutationRate = mutationRate;
        _selectionThreshold = selectionThreshold;
        _creatureGenerator = creatureGenerator;
        _soundController = soundController;
    }

    /// <summary>
    /// Fait évoluer une population de créatures sur un nombre donné de générations
    /// </summary>
    /// <param name="generations">Nombre de générations à simuler</param>
    /// <param name="actualGeneration">Population de créatures actuelle</param>
    /// <returns>Nouvelle population après évolution</returns>
    public List<Creature> EvolvePopulation(int generations, List<Creature> actualGeneration)
    {
        int iteration = 0;

        while (iteration < generations)
        {
            // Créer une nouvelle liste pour stocker la population de cette itération
            List<Creature> newGenerationPopulation = new List<Creature>();

            // Sélection séparée pour les populations de Forêt et de Désert
            List<Creature> selectedForestPopulation = Selection(actualGeneration, CreatureType.Forest);
            List<Creature> selectedDesertPopulation = Selection(actualGeneration, CreatureType.Desert);

            // Faire évoluer chaque population séparément
            List<Creature> newForestPopulation = EvolveSpecificPopulation(selectedForestPopulation);
            List<Creature> newDesertPopulation = EvolveSpecificPopulation(selectedDesertPopulation);

            // Combiner les nouvelles populations
            newGenerationPopulation.AddRange(newForestPopulation);
            newGenerationPopulation.AddRange(newDesertPopulation);

            // Mettre à jour la génération actuelle pour la prochaine itération
            actualGeneration = newGenerationPopulation;

            iteration++;
        }

        return actualGeneration;
    }

    /// <summary>
    /// Sélectionne les créatures les plus aptes pour la reproduction
    /// </summary>
    /// <param name="actualGeneration">Population actuelle</param>
    /// <param name="creatureType">Type de créature à sélectionner</param>
    /// <returns>Liste des créatures sélectionnées</returns>
    private List<Creature> Selection(List<Creature> actualGeneration, CreatureType creatureType)
    {
        List<Creature> selectedPopulation = new List<Creature>();

        // Filtrer la population pour ne garder que les créatures du type spécifié
        List<Creature> filteredPopulation = actualGeneration.Where(c => c.Type == creatureType).ToList();

        // Trier par fitness décroissante
        filteredPopulation.Sort((a, b) => b.fitness.CompareTo(a.fitness));

        foreach (var creature in filteredPopulation)
        {
            // Calcul de la chance de sélection avec une part d'aléatoire
            float selectionChance = creature.fitness + Random.Range(0f, 1f) * creature.fitness * 0.1f;
            if (selectionChance >= (filteredPopulation[0].fitness * _fitnessImportanceBias))
            {
                selectedPopulation.Add(creature);
            }

            // Limiter le nombre de créatures sélectionnées
            if (selectedPopulation.Count >= (int)(filteredPopulation.Count * _selectionThreshold))
            {
                break;
            }
        }

        return selectedPopulation;
    }

    /// <summary>
    /// Fait évoluer une population spécifique de créatures
    /// </summary>
    /// <param name="selectedPopulation">Population sélectionnée pour reproduction</param>
    /// <returns>Nouvelle génération de créatures</returns>
    private List<Creature> EvolveSpecificPopulation(List<Creature> selectedPopulation)
    {
        List<Creature> newPopulation = new List<Creature>();

        while (newPopulation.Count < selectedPopulation.Count / 2)
        {
            // Sélection aléatoire des parents
            Creature parent1 = selectedPopulation[Random.Range(0, selectedPopulation.Count)];
            Creature parent2 = selectedPopulation[Random.Range(0, selectedPopulation.Count)];

            // Création d'un enfant par recombinaison
            Creature child = Recombination(parent1, parent2);

            // Mutation de l'enfant
            Mutate(child);

            newPopulation.Add(child);
        }

        return newPopulation;
    }

    /// <summary>
    /// Crée une nouvelle créature par recombinaison de deux parents
    /// </summary>
    /// <param name="parent1">Premier parent</param>
    /// <param name="parent2">Deuxième parent</param>
    /// <returns>Créature issue de la recombinaison</returns>
    private Creature Recombination(Creature parent1, Creature parent2)
    {
        // Vérifier que les parents sont du même type
        if (parent1.Type != parent2.Type)
        {
            return null;
        }

        // Création du génome de l'enfant par croisement
        List<int> genome = new List<int>(new int[parent1.genomeLength]);
        int crossoverPoint = Random.Range(1, (parent1.genomeLength / 2) + 1);

        for (int i = 0; i < parent1.genomeLength; i++)
        {
            genome[i] = (i < crossoverPoint) ? parent1.genome[i] : parent2.genome[i];
        }

        return new Creature(genome, _creatureGenerator, _soundController);
    }

    /// <summary>
    /// Applique une mutation sur le génome d'une créature
    /// </summary>
    /// <param name="creature">Créature à muter</param>
    private void Mutate(Creature creature)
    {
        for (int i = 0; i < creature.genome.Count; i++)
        {
            // Muter un gène selon le taux de mutation
            if (Random.Range(0f, 1f) < _mutationRate)
            {
                creature.genome[i] = 1 - creature.genome[i];  // Inverser le bit
            }
        }
    }
}