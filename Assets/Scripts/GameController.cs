using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contrôle principal du déroulement du jeu et de ses différentes phases
/// </summary>
public class GameController : MonoBehaviour 
{
    // Références aux différents contrôleurs du jeu
    public CreatureGenerator creatureGenerator;  // Générateur de créatures
    public SoundController soundController;      // Contrôleur sonore
    public FoodController foodController;        // Contrôleur de nourriture
    public TerrainController terrainController;  // Contrôleur de terrain

    public int beginningCreatureNumber = 30;     // Nombre initial de créatures
    public float mutationRate = 0.04f;        // Taux de mutation
    public float selectionThreshold = 0.7f;   // Taux de sélection

    private Population _creaturesPopulation;     // Population de créatures
    private GameState _state = GameState.Collapsing;  // État actuel du jeu

    /// <summary>
    /// Mise à jour frame par frame de l'état du jeu
    /// </summary>
    void Update()
    {
        switch (_state)
        {
            // Phase de génération du terrain
            case GameState.Collapsing:
                if (terrainController.IsCollapsed)
                {
                    _state = GameState.Starting;
                }
                break;

            // Phase d'initialisation du jeu
            case GameState.Starting:
                Begin();
                _state = GameState.Living;
                break;

            // Phase de simulation active
            case GameState.Living:
                Live();
                break;
        }
    }

    /// <summary>
    /// Initialise la population de créatures et la nourriture
    /// </summary>
    void Begin()
    {
        // Créer la population initiale de créatures
        _creaturesPopulation = new Population(
            creatureGenerator, soundController, beginningCreatureNumber, mutationRate, selectionThreshold);
        
        // Faire évoluer la population initiale
        _creaturesPopulation.Evolve(1);
        
        // Configurer le contrôleur de nourriture
        foodController.SetPopulation(_creaturesPopulation);
        foodController.SpawnInitialFood();
    }

    /// <summary>
    /// Gère la mise à jour continue de la population et de la faim des créatures
    /// </summary>
    void Live()
    {
        // Mettre à jour la population
        _creaturesPopulation.Update();

        // Gérer la faim de chaque créature
        foreach (var creature in _creaturesPopulation.Members)
        {
            foodController.UpdateCreatureHunger(creature);
        }
    }
}

/// <summary>
/// États possibles du jeu
/// </summary>
public enum GameState 
{
    Collapsing,  // Génération du terrain
    Starting,    // Initialisation
    Living,      // Simulation active
}