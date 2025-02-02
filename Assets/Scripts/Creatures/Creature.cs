using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Creature
{
    // Génome de la créature : liste de bits définissant ses traits génétiques
    public List<int> genome;
    public float fitness;              // Score de fitness évaluant la qualité globale
    public GameObject model;           // Modèle 3D représentant la créature
    public Collider creatureCollider;  // Composant de détection de collision
    public CreatureGenerator creatureGenerator;  // Générateur de modèles
    public SoundController soundController;
    public int genomeLength = 21;      // Nombre total de bits dans le génome

    // Statistiques vitales du cycle de vie
    public float pv;                   // Points de vie
    public float faim;                 // Niveau de faim

    // Attributs privés décodés du génome
    private CreatureType _type;
    private Color _color;               // Couleur visuelle 
    private int _tentaclesWidth;        // Longueur des tentacules 
    private int _numberOfTentacles;     // Nombre total de tentacules
    private float _scaleFactor;         // Multiplicateur de taille
    private int _numberOfMoustaches;    // Nombre de moustaches
    private int _typeOfMoustache;       // Style de moustache
    private int _typeOfHorns;           // Configuration potentielle de cornes
    public float _speed;                // Vitesse de déplacement

    // Accesseurs pour les attributs décodés du génome
    public CreatureType Type => _type;
    public Color Color => _color;
    public int TentaclesWidth => _tentaclesWidth;
    public int NumberOfTentacles => _numberOfTentacles;
    public int NumberOfMoustaches => _numberOfMoustaches;
    public int TypeOfMoustache => _typeOfMoustache;
    public int TypeOfHorns => _typeOfHorns;
    public float ScaleFactor => _scaleFactor;
    public float Speed => _speed;

    /// <summary>
    /// Constructeur pour créer une créature avec un génome aléatoire d'une longueur donnée
    /// Décode le génome et génère un modèle 3D 
    /// </summary>
    /// <param name="genomeLength">Longueur du génome (en bits)</param>
    /// <param name="generator">Référence au générateur de modèles</param>
    public Creature(CreatureGenerator generator, SoundController soundController, CreatureType type)
    {
        genome = new List<int>();

        // Premier bit détermine le type (0 pour Forêt, 1 pour Désert)
        int bitType = (type == CreatureType.Forest) ? 0 : 1;
        genome.Add(bitType);

        // Remplir les bits restants avec des 0 et 1 aléatoires
        for (int i = 1; i < genomeLength; i++)
        {
            genome.Add(Random.Range(0, 2));
        }

        CreatureCommons(generator, soundController);
    }

    /// <summary>
    /// Constructeur pour créer une créature à partir d'un génome déjà existant
    /// Décode le génome et génère un modèle 3D
    /// </summary>
    /// <param name="generatedGenome">Génome prédéfini</param>
    /// <param name="generator">Référence au générateur de modèles</param>
    public Creature(List<int> generatedGenome, CreatureGenerator generator, SoundController soundController)
    {
        genome = generatedGenome;
        CreatureCommons(generator, soundController);
    }

    /// <summary>
    /// Methodes communes des constructeurs de creatures
    /// </summary>
    /// <param name="generator">Référence au générateur de modèles</param>
    private void CreatureCommons(CreatureGenerator generator, SoundController controller)
    {
        creatureGenerator = generator;

        DecodeGenome();         // Convertir le génome binaire en attributs
        EvaluateFitness();       // Calculer le potentiel adaptatif
        model = creatureGenerator.GenerateModel(this);  // Créer la représentation 3D

        // Ajouter des capacités sonores au modèle
        soundController = model.AddComponent<SoundController>();
        soundController.StartCoroutine(InitializeAudioAfterStart(controller));

        // Ajouter des capacités de mouvement
        CreatureMovement movementScript = model.AddComponent<CreatureMovement>();
        movementScript.Initialize(this);

        // Configurer les interactions physiques (Collider et Rigidbody)
        AddColliderToModel();
        AddRigidbodyToModel();
    }

    private IEnumerator InitializeAudioAfterStart(SoundController controller)
    {
        // Attendre la fin de la frame pour être sûr que Start a été appelé 
        yield return new WaitForEndOfFrame();

        soundController.bornSound = controller.bornSound;
        soundController.eatingSound = controller.eatingSound;
        soundController.deathSound = controller.deathSound;

        soundController.PlayBornSound();
    }

    /// <summary>   
    /// Cycle de la vie, descend les pv de la créature en fonction du temps qui passe 
    /// </summary>
    public void UpdatePv()
    {
        pv -= 2f * Time.deltaTime;
    }

    /// <summary>
    /// Mort de la créature, détruit son model 3D 
    /// </summary>
    public void Die()
    {
        if (model != null)
        {
            soundController.PlayDeathSound();
            UnityEngine.Object.Destroy(model);
        }
    }

    /// <summary>   
    /// Cycle de la vie, descend les pv de la créature en fonction du temps qui passe 
    /// </summary>
    public void Eat(float hunger)
    {
        faim = Mathf.Min(faim + hunger, 100f); // cap à 100
        soundController.PlayEatingSound();
    }


    private void AddColliderToModel()
    {
        // Ajouter un Collider de type CapsuleCollider au modèle
        BoxCollider collider = model.AddComponent<BoxCollider>();

        // Ajuster la taille du collider en fonction du facteur d'échelle
        float scaleFactor = ScaleFactor;
        collider.size = new Vector3(4f, 12f, 2f);
        collider.center = new Vector3(0, -0.35f, 0);

        // Rendre le collider comme un trigger pour les interactions
        collider.isTrigger = false;

        // Stocker la référence du Collider
        creatureCollider = collider;
    }

    private void AddRigidbodyToModel()
    {
        Rigidbody rb = model.AddComponent<Rigidbody>();
        rb.mass = ScaleFactor; // Adjust mass based on scale
        rb.drag = 0.5f;
        rb.angularDrag = 0.5f;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }


    /// <summary>
    /// Décoder le génome pour obtenir les attributs physiques de la créature (couleur, tentacules, taille)
    /// </summary>
    private void DecodeGenome()
    {
        _type = DecodeCreatureType();
        _color = DecodeColor();
        _tentaclesWidth = DecodeTentaclesWidth();
        _numberOfTentacles = DecodeTentaclesNumber();
        _scaleFactor = DecodeScaleFactor();
        _numberOfMoustaches = DecodeMoustacheNumber();
        _typeOfMoustache = DecodeMoustacheType();
        _speed = DecodeSpeed();
        faim = DecodeHunger();
        pv = DecodePV();
    }

    /// <summary>
    /// Évaluer la fitness de la créature en fonction de ses attributs (sa couleur,  sa taille, et le nombre, la longuer et le _type de courbe des tentacules)
    /// </summary>
    private void EvaluateFitness()
    {
        fitness = 0f;
        fitness += EvaluateColor();
        fitness += EvaluateScale();
        fitness += EvaluateTentaclesWidth();
        fitness += EvaluateTentaclesNumber();
        fitness += EvaluateCourbe();
        fitness += EvaluateMoustacheType();
        fitness += EvaluateMoustacheNumber();
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
            case (3): return (_type == CreatureType.Forest) ? 3f : 2f;
            case (2): return (_type == CreatureType.Forest) ? 2.75f : 2.5f;
            case (1): return (_type == CreatureType.Forest) ? 2.5f : 2.5f;
            case (0): return (_type == CreatureType.Forest) ? 1.75f : 3f;
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
            case (3): return (_type == CreatureType.Forest) ? 3f : 1.75f;
            case (2): return (_type == CreatureType.Forest) ? 2.5f : 2.5f;
            case (1): return (_type == CreatureType.Forest) ? 2.5f : 2.75f;
            case (0): return (_type == CreatureType.Forest) ? 2f : 3f;
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
            case (3): return (_type == CreatureType.Forest) ? 2f : 1.5f;
            case (2): return (_type == CreatureType.Forest) ? 3.5f : 3f;
            case (1): return (_type == CreatureType.Forest) ? 3f : 3.5f;
            case (0): return (_type == CreatureType.Forest) ? 1.5f : 2f;
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
            case (3): return (_type == CreatureType.Forest) ? 1.75f : 1.75f;
            case (2): return (_type == CreatureType.Forest) ? 3f : 3f;
            case (1): return (_type == CreatureType.Forest) ? 3.5f : 3.5f;
            case (0): return (_type == CreatureType.Forest) ? 1.75f : 1.75f;
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
            case (3): return (_type == CreatureType.Forest) ? 3f : 3f;
            case (2): return (_type == CreatureType.Forest) ? 1.5f : 1.5f;
            case (1): return (_type == CreatureType.Forest) ? 2.5f : 2.5f;
            case (0): return (_type == CreatureType.Forest) ? 3.5f : 3.5f;
            default: return 0;
        }
    }

    /// <summary>
    /// Évalue le nombre de moustaches  
    /// </summary>
    /// <returns>Score de fitness basé sur le nombre de moustaches</returns>
    private float EvaluateMoustacheNumber()
    {
        int moustacheBits = Utils.BitToInt(genome[11], genome[12]);
        switch (moustacheBits)
        {
            case (3): return (_type == CreatureType.Forest) ? 3f : 3f;
            case (2): return (_type == CreatureType.Forest) ? 3f : 3f;
            case (1): return (_type == CreatureType.Forest) ? 2f : 2f;
            case (0): return (_type == CreatureType.Forest) ? 2.5f : 2.5f;
            default: return 0;
        }
    }

    /// <summary>
    /// Évalue le style des moustaches  
    /// </summary>
    /// <returns>Score de fitness basé sur le type de moustache</returns>
    private float EvaluateMoustacheType()
    {
        int moustacheBits = Utils.BitToInt(genome[13], genome[14]);
        switch (moustacheBits)
        {
            case (3): return (_type == CreatureType.Forest) ? 2f : 2f;
            case (2): return (_type == CreatureType.Forest) ? 2.5f : 2.5f;
            case (1): return (_type == CreatureType.Forest) ? 3.5f : 3.5f;
            case (0): return (_type == CreatureType.Forest) ? 3f : 3f;
            default: return 0;
        }
    }

    /// <summary>
    /// Évalue le style des moustaches  
    /// </summary>
    /// <returns>Score de fitness basé sur le type de moustache</returns>
    private float EvaluateSpeed()
    {
        int speedBits = Utils.BitToInt(genome[19], genome[20]);
        switch (speedBits)
        {
            case (3): return (_type == CreatureType.Forest) ? 2f : 3.5f;
            case (2): return (_type == CreatureType.Forest) ? 2.5f : 3f;
            case (1): return (_type == CreatureType.Forest) ? 3.5f : 2.5f;
            case (0): return (_type == CreatureType.Forest) ? 3.5f : 2f;
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

    /// <summary>
    /// Décode le nombre de moustache à partir des bits du génome
    /// </summary>
    /// <returns>Nombre de moustache</returns>
    private int DecodeMoustacheNumber()
    {
        return Utils.BitToInt(genome[11], genome[12]);
    }

    /// <summary>
    /// Décode le type de moustache à partir des bits du génome
    /// </summary>
    /// <returns>Type de moustache</returns>
    private int DecodeMoustacheType()
    {
        return Utils.BitToInt(genome[13], genome[14]);
    }

    /// <summary>
    /// Décode le nombre de PV de la créature à partir des bits du génome
    /// </summary>
    /// <returns>PV max de la créature</returns>
    private float DecodePV()
    {
        float pvBits = Utils.BitToInt(genome[15], genome[16]);
        float mappedValue = Mathf.Lerp(60f, 100f, Mathf.InverseLerp(0, 3, pvBits));
        return mappedValue;
    }

    /// <summary>
    /// Décode le nombre de PV de la créature à partir des bits du génome
    /// </summary>
    /// <returns>PV max de la créature</returns>
    private float DecodeHunger()
    {
        float hungerBits = Utils.BitToInt(genome[17], genome[18]);
        float mappedValue = Mathf.Lerp(60f, 100f, Mathf.InverseLerp(0, 3, hungerBits));
        return mappedValue;
    }

    /// <summary>
    /// Décode la vitesse de déplacement de la créature à partir des bits du génome
    /// </summary>
    /// <returns>_speed de la créature</returns>
    private float DecodeSpeed()
    {
        float speedBits = Utils.BitToInt(genome[19], genome[20]);
        return (speedBits + 5f) * 1.2f;
    }



}