using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ForestCreature
{
    public List<int> genome;           // Liste d'entier (0 ou 1) représentant le génome de la créature (suite de bits)
    public float fitness;              // Fitness de la créature (évaluation de sa qualité)
    public GameObject model;           // Modèle 3D associé à la créature
    public ForestCreatureGenerator creatureGenerator;  // Référence du générateur de modèles
    public int genomeLength = 10;

    private Color color;               // Couleur de la créature
    private int tentaclesWidth;        // Longueur des tentacules 
    private int numberOfTentacles;     // Nombre de tentacules
    private float scaleFactor;         // Facteur de la taille globale de la créature



    /// <summary>
    /// Constructeur pour créer une créature avec un génome aléatoire d'une longueur donnée
    /// Décode le génome et génère un modèle 3D
    /// </summary>
    /// <param name="genomeLength">Longueur du génome (en bits)</param>
    /// <param name="generator">Référence au générateur de modèles</param>
    public ForestCreature(int genomeLength, ForestCreatureGenerator generator)
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
    public ForestCreature(List<int> generatedGenome, ForestCreatureGenerator generator)
    {
        genome = generatedGenome;                      
        creatureGenerator = generator;

        DecodeGenome();       
        EvaluateFitness();       
        model = creatureGenerator.GenerateModel(this); 
    }


    public Color Color
    {
        get { return color; }  
    }
    public int TentaclesWidth
    {
        get { return tentaclesWidth; } 
    }
    public int NumberOfTentacles
    {
        get { return numberOfTentacles; }  
    }
    public float ScaleFactor
    {
        get { return scaleFactor; }
    }


    /// <summary>
    /// Décoder le génome pour obtenir les attributs physiques de la créature (couleur, tentacules, taille)
    /// </summary>
    private void DecodeGenome()
    {
        color = DecodeColor(); 
        tentaclesWidth = DecodeTentaclesWidth();
        numberOfTentacles = DecodeTentaclesNumber();
        scaleFactor = DecodeScaleFactor();
    }

    /// <summary>
    /// Évaluer la fitness de la créature en fonction de ses attributs (sa couleur,  sa taille, et le nombre, la longuer et le type de courbe des tentacules)
    /// </summary>
    private void EvaluateFitness()
    {
        fitness = 0f;
        fitness += EvaluateColor();
        fitness += EvaluateScale();
        fitness += EvaluateTentaclesWidth();
        fitness += EvaluateTentaclesNumber();
        fitness += EvaluateCourbe();
    }

    /// <summary>
    /// Évalue la couleur de la créature 
    /// </summary>
    /// <returns>Score de fitness basé sur la couleur</returns>
    private float EvaluateColor()
    {
        int colorBits = Utils.BitToInt(genome[0], genome[1]); 
        switch(colorBits)
        {
            case(3): return 1.75f;         
            case(2): return 2.5f;       
            case(1): return 2.25f;       
            case(0): return 3.5f;
            default: return 0;          
        }
    }

    /// <summary>
    /// Évalue la taille de la créature 
    /// </summary>
    /// <returns>Score de fitness basé sur la taille de la créature</returns>
    private float EvaluateScale()
    {
        int scaleBits = Utils.BitToInt(genome[6], genome[7]); 
        switch(scaleBits)
        {
            case(3): return 1.5f;      
            case(2): return 2.75f;
            case(1): return 3.25f;
            case(0): return 2.5f;
            default: return 0;
        }
    }

    /// <summary>
    /// Évalue la longueur des tentacules 
    /// </summary>
    /// <returns>Score de fitness basé sur la longueur des tentacules</returns>
    private float EvaluateTentaclesWidth()
    {
        int tentaclesWidthBits = Utils.BitToInt(genome[2], genome[3]);  
        switch(tentaclesWidthBits)
        {
            case(3): return 1.5f;     
            case(2): return 3f;       
            case(1): return 2.5f;
            case(0): return 2f;
            default: return 0;
        }
    }

    /// <summary>
    /// Évaluer le nombre des tentacules 
    /// </summary>
    /// <returns>Score de fitness basé sur la nombre de tentacules</returns>
    private float EvaluateTentaclesNumber()
    {
        int legsBits = Utils.BitToInt(genome[4], genome[5]); 
        switch(legsBits)
        {
            case(3): return 1.75f;     
            case(2): return 3f;       
            case(1): return 3.5f;
            case(0): return 1.75f;
            default: return 0;
        }
    }

    /// <summary>
    /// Évalue la courbe des tentacules 
    /// </summary>
    /// <returns>Score de fitness basé sur la forme de la courbe des tentacules</returns>
    private float EvaluateCourbe()
    {
        int courbeBits = Utils.BitToInt(genome[8], genome[9]);
        switch(courbeBits)
        {
            case(3): return 3f;     
            case(2): return 1.5f;
            case(1): return 2.5f;
            case(0): return 3.5f;
            default: return 0;
        }
    }

    /// <summary>
    /// Décode la couleur de la créature à partir des bits du génome
    /// </summary>
    /// <returns>Couleur de la créature</returns>
    private Color DecodeColor()
    {
        int colorBits = Utils.BitToInt(genome[0], genome[1]);
        switch (colorBits)
        {
            case 0: return Utils.GetNavajoWhite();
            case 1: return Utils.GetLightSlateGray();
            case 2: return Utils.GetLightSkyBlue();
            case 3: return Utils.GetMediumSeaGreen();
            default: return Color.white;
        }
    }

    /// <summary>
    /// Décode la longueur des tentacules à partir des bits du génome
    /// </summary>
    /// <returns>Longueur en nombre de spères des tentacules </returns>
    private int DecodeTentaclesWidth()
    {
        int tentaclesWidthBits = Utils.BitToInt(genome[2], genome[3]); 
        switch (tentaclesWidthBits)
        {
            case 0: return 8;
            case 1: return 12;
            case 2: return 17;
            case 3: return 20;
            default: return 0;
        }
    }

    /// <summary>
    /// Décode le nombre de tentacules à partir des bits du génome
    /// </summary>
    /// <returns>Nombre de tentacules</returns>
    private int DecodeTentaclesNumber()
    {
        int legsBits = Utils.BitToInt(genome[4], genome[5]); 
        return (legsBits + 1) * 3; // Nombre de tentacules : 3, 6, 9 ou 12
    }

    /// <summary>
    /// Décode le facteur d'échelle à partir des bits du génome
    /// </summary>
    /// <returns>Facteur de taille</returns>
    private int DecodeScaleFactor()
    {
        int scaleBits = Utils.BitToInt(genome[6], genome[7]); 
        return scaleBits + 1; 
    }

    /// <summary>
    /// Décode la courbe des tentacules pour déterminer la forme du mouvement
    /// </summary>
    /// <param name="t">Emplecement du point sur la courbe</param>
    /// <returns>Coordonnées de l'empalcement t</returns>
    public Vector3 DecodeCourbe(float t, float tentacleLength)
    {
        int courbeBits = Utils.BitToInt(genome[8], genome[9]);
        float locationFactor = t/tentacleLength; 
        switch (courbeBits)
        {
            case 0: return new Vector3(locationFactor*Mathf.Sin(t * 2), t, locationFactor*Mathf.Cos(t * 2)); // Courbe hélicoïdale
            case 1: return new Vector3(locationFactor*Mathf.Sin(t), t, 0); // Courbe sinusoïdale simple
            case 2: return new Vector3(0, t, locationFactor*Mathf.Cos(t)); // Courbe sur l'axe Z
            case 3: return new Vector3(locationFactor*Mathf.Sin(t), t, locationFactor*Mathf.Sin(t)); // Double sinus
            default: return new Vector3(0, t, 0); // Ligne droite
        }
    }

}
