using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CreatureGenerator creatureGenerator; // Référence au générateur de créatures

    public SoundController soundController; // Référence au controller sonore

    public FoodController foodController;  // Référence au FoodController

    public TerrainController terrainController; // Référence au TerrainController

    public int beginningCreatureNumber = 30; // Nombre de créatures dans la population de départ 

    private Population _creaturesPopulation;

    private GameState _state = GameState.Collapsing;

    void Update()
    {
        switch (_state)
        {
            case GameState.Collapsing:
                if (terrainController.IsCollapsed)
                {
                    _state = GameState.Starting;
                }
                break;
            case GameState.Starting:
                Begin();
                _state = GameState.Living;
                break;
            case GameState.Living:
                Live();
                break;
        }

    }

    void Begin()
    {
        _creaturesPopulation = new Population(
            creatureGenerator, soundController, beginningCreatureNumber);
        _creaturesPopulation.Evolve(1);
        foodController.SetPopulation(_creaturesPopulation);
        foodController.SpawnInitialFood();
    }

    void Live()
    {
        _creaturesPopulation.Update();
        // FoodController foodController = FindObjectOfType<FoodController>();
        foreach (var creature in _creaturesPopulation.Members)
        {
            foodController.UpdateCreatureHunger(creature);
        }
    }
}

public enum GameState
{
    Collapsing,
    Starting,
    Living,
};
