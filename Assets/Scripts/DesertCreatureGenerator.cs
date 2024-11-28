// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class DesertCreatureGenerator<DesertCreature> : CreatureGenerator<DesertCreature> 
// {
//     public GameObject eyePrefab;    // Prefab pour les yeux 
//     public GameObject spherePrefab; // Prefab pour les sphères du corps

//     private Vector3 initialPosition;
    

//     /// <summary>
//     /// Génère un modèle 3D à partir du génome d'une créature
//     /// </summary>
//     /// <param name="creature">La créature dont le modèle doit être généré</param>
//     /// <returns>Le modèle GameObject généré</returns>
//     public override GameObject GenerateModel(DesertCreature creature)
//     {
//         GameObject creatureModel = new GameObject("CreatureModel");
//         this.initialPosition = new Vector3(Random.Range(0, 50), Random.Range(0, 10), Random.Range(0, 50));
//         creatureModel.transform.position = initialPosition;
//         return creatureModel;
//     }



// }
