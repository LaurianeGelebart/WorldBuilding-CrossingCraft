using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Creature
{
    public List<int> genome;           // Liste d'entier (0 ou 1) représentant le génome de la créature (suite de bits)
    public float fitness;              // Fitness de la créature (évaluation de sa qualité)
    public GameObject model;           // Modèle 3D associé à la créature
    public CreatureGenerator creatureGenerator;  // Référence du générateur de modèles
    public int genomeLength = 15;
    public CreatureType creatureType;

    public float pv;
    public float faim;


    private Color _color;               // Couleur de la créature
    private int _tentaclesWidth;        // Longueur des tentacules 
    private int _numberOfTentacles;     // Nombre de tentacules
    private float _scaleFactor;         // Facteur de la taille globale de la créature
    private int _numberOfMoustaches;
    private int _typeOfMoustache;
    private int _typeOfCornes; 

    public Color Color => _color;
    public int TentaclesWidth => _tentaclesWidth;
    public int NumberOfTentacles => _numberOfTentacles;
    public int NumberOfMoustaches => _numberOfMoustaches;
    public int TypeOfMoustache => _typeOfMoustache;
    public int TypeOfCornes => _typeOfCornes;
    public float ScaleFactor => _scaleFactor;


    /// <summary>
    /// Constructeur pour créer une créature avec un génome aléatoire d'une longueur donnée
    /// Décode le génome et génère un modèle 3D 
    /// </summary>
    /// <param name="genomeLength">Longueur du génome (en bits)</param>
    /// <param name="generator">Référence au générateur de modèles</param>
    public Creature(CreatureGenerator generator)
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
    public Creature(List<int> generatedGenome, CreatureGenerator generator)
    {
        genome = generatedGenome;
        creatureGenerator = generator;

        DecodeGenome();
        EvaluateFitness();
        model = creatureGenerator.GenerateModel(this);
    }



    /// <summary>
    /// Décoder le génome pour obtenir les attributs physiques de la créature (couleur, tentacules, taille)
    /// </summary>
    private void DecodeGenome()
    {
        creatureType = DecodeCreatureType();
        _color = DecodeColor();
        _tentaclesWidth = DecodeTentaclesWidth();
        _numberOfTentacles = DecodeTentaclesNumber();
        _scaleFactor = DecodeScaleFactor();
    }

    /// <summary>
    /// Évaluer la fitness de la créature en fonction de ses attributs (sa couleur,  sa taille, et le nombre, la longuer et le creatureType de courbe des tentacules)
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
        int colorBits = Utils.BitToInt(genome[1], genome[2]);
        switch (colorBits)
        {
            case (3): return 1.75f;
            case (2): return 2.5f;
            case (1): return 2.25f;
            case (0): return 3.5f;
            default: return 0;
        }
    }

    /// <summary>
    /// Évalue la taille de la créature 
    /// </summary>
    /// <returns>Score de fitness basé sur la taille de la créature</returns>
    private float EvaluateScale()
    {
        int scaleBits = Utils.BitToInt(genome[7], genome[8]);
        switch (scaleBits)
        {
            case (3): return 1.5f;
            case (2): return 2.75f;
            case (1): return 3.25f;
            case (0): return 2.5f;
            default: return 0;
        }
    }

    /// <summary>
    /// Évalue la longueur des tentacules 
    /// </summary>
    /// <returns>Score de fitness basé sur la longueur des tentacules</returns>
    private float EvaluateTentaclesWidth()
    {
        int tentaclesWidthBits = Utils.BitToInt(genome[3], genome[4]);
        switch (tentaclesWidthBits)
        {
            case (3): return 1.5f;
            case (2): return 3f;
            case (1): return 2.5f;
            case (0): return 2f;
            default: return 0;
        }
    }

    /// <summary>
    /// Évaluer le nombre des tentacules 
    /// </summary>
    /// <returns>Score de fitness basé sur la nombre de tentacules</returns>
    private float EvaluateTentaclesNumber()
    {
        int legsBits = Utils.BitToInt(genome[6], genome[7]);
        switch (legsBits)
        {
            case (3): return 1.75f;
            case (2): return 3f;
            case (1): return 3.5f;
            case (0): return 1.75f;
            default: return 0;
        }
    }

    /// <summary>
    /// Évalue la courbe des tentacules 
    /// </summary>
    /// <returns>Score de fitness basé sur la forme de la courbe des tentacules</returns>
    private float EvaluateCourbe()
    {
        int courbeBits = Utils.BitToInt(genome[9], genome[10]);
        switch (courbeBits)
        {
            case (3): return 3f;
            case (2): return 1.5f;
            case (1): return 2.5f;
            case (0): return 3.5f;
            default: return 0;
        }
    }

    /// <summary>
    /// Décode le type de la créature à partir des bits du génome
    /// Type possible CreatureType.Desert CreatureType.Forest 
    /// </summary>
    /// <returns>Type de la créature</returns>
    private CreatureType DecodeCreatureType()
    {
        switch (genome[0])
        {
            case 0: return CreatureType.Forest;
            case 1: return CreatureType.Desert;
            default: return CreatureType.Desert;
        }
    }

    /// <summary>
    /// Décode la couleur de la créature à partir des bits du génome
    /// </summary>
    /// <returns>Couleur de la créature</returns>
    private Color DecodeColor()
    {
        int colorBits = Utils.BitToInt(genome[1], genome[2]);
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
        int tentaclesWidthBits = Utils.BitToInt(genome[3], genome[4]);
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
        int legsBits = Utils.BitToInt(genome[6], genome[7]);
        return (legsBits + 2) * 2; 
    }

    /// <summary>
    /// Décode le facteur d'échelle à partir des bits du génome
    /// </summary>
    /// <returns>Facteur de taille</returns>
    private int DecodeScaleFactor()
    {
        int scaleBits = Utils.BitToInt(genome[7], genome[8]);
        return scaleBits + 1;
    }

    /// <summary>
    /// Décode la courbe des tentacules pour déterminer la forme du mouvement
    /// </summary>
    /// <param name="t">Emplacement du point sur la courbe</param>
    /// <returns>Coordonnées de l'emplacement t</returns>
    public Vector3 DecodeCourbe(float t, float tentacleLength)
    {
        int courbeBits = Utils.BitToInt(genome[9], genome[10]);
        float locationFactor = t / tentacleLength;
        switch (courbeBits)
        {
            case 0: return new Vector3(locationFactor * Mathf.Sin(t * 2), t, locationFactor * Mathf.Cos(t * 2)); // Courbe hélicoïdale
            case 1: return new Vector3(locationFactor * Mathf.Sin(t), t, 0); // Courbe sinusoïdale simple
            case 2: return new Vector3(0, t, locationFactor * Mathf.Cos(t)); // Courbe sur l'axe Z
            case 3: return new Vector3(locationFactor * Mathf.Sin(t), t, locationFactor * Mathf.Sin(t)); // Double sinus
            default: return new Vector3(0, t, 0); // Ligne droite
        }
    }

}