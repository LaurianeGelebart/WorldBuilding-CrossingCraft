using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class CreatureGenerator<Creature> : MonoBehaviour 
{
    private Vector3 initialPosition;
    
    public abstract GameObject GenerateModel(Creature creature);

}
