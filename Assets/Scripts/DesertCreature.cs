// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class DesertCreature : Creature
// {
//     public DesertCreature(DesertCreatureGenerator<DesertCreature>  generator) 
//         : base(generator) 
//     {
//         // Logique spécifique pour le désert
//         Debug.Log("Créature du désert créée");
//     }

//     public DesertCreature(List<int> generatedGenome, DesertCreatureGenerator<DesertCreature>  generator) 
//         : base(generatedGenome, generator) 
//     {
//         Debug.Log("Créature du désert créée avec un génome prédéfini");
//     }

//     // Vous pouvez surcharger DecodeGenome ou EvaluateFitness si nécessaire
//     public override void DecodeGenome()
//     {
       
//     }

//     public override void EvaluateFitness()
//     {
//         base.EvaluateFitness();
//         fitness += 2f;  // Exemple de personnalisation
//     }

//     public override void Update()
//     {
        
//     }
// }
