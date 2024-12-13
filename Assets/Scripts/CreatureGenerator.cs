using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureGenerator : MonoBehaviour
{
    public GameObject eyePrefab;    // Prefab pour les yeux 
    public GameObject spherePrefab; // Prefab pour les sphères du corps
    public GameObject moustache1Prefab; // Prefab pour les moustaches
    public GameObject moustache2Prefab; // Prefab pour les moustaches
    public GameObject moustache3Prefab; // Prefab pour les moustaches
    public GameObject moustache4Prefab; // Prefab pour les moustaches
    public GameObject hornPrefab; // Prefab pour les cornes 
    public GameObject cubePrefab; // Prefab pour les cube du corps
    private Vector3 _initialPosition;

    /// <summary>
    /// Génère un modèle 3D à partir du génome d'une créature
    /// </summary>
    /// <param name="creature">La créature dont le modèle doit être généré</param>
    /// <returns>Le modèle GameObject généré</returns>
    public GameObject GenerateModel(Creature creature)
    {
        GameObject creatureModel = new GameObject("CreatureModel");
        _initialPosition = new Vector3(Random.Range(-200, 200), Random.Range(10, 20), Random.Range(-200, 200));
        creatureModel.transform.position = _initialPosition;

        Vector3 headPosition = CreateBody(creatureModel, creature);
        CreateEyes(headPosition, creatureModel, creature);
        CreateMoustache(headPosition, creatureModel, creature);
        CreateTentacles(creatureModel, creature);
        if (creature.Type == CreatureType.Desert) CreateHorns(headPosition, creatureModel, creature);

        return creatureModel;
    }

    /// <summary>
    /// Génère les sphères du corps et retourne la position de la dernière sphère (tête)
    /// </summary>
    /// <param name="creatureModel">Le modèle de créature à faire évoluer</param>
    /// <param name="creature">La créature dont le modèle est en cours de création</param>
    /// <returns>La position de la dernière sphère (tête)</returns>
    private Vector3 CreateBody(GameObject creatureModel, Creature creature)
    {
        float scaleFactor = creature.ScaleFactor;
        Color creatureColor = creature.Color;

        // Tailles des sphères du corps basées sur le scaleFactor contenu dans le genome
        float baseSize = 0.5f * scaleFactor;
        float largeSize = 1.2f * scaleFactor;
        float smallerSize = 0.7f * scaleFactor;
        float topSize = 0.3f * scaleFactor;

        // Positionement de chaque sphère
        Vector3 basePosition = _initialPosition;
        Vector3 largePosition = basePosition + new Vector3(0f, (baseSize / 2 + largeSize / 2) * 0.7f, 0);
        Vector3 smallerPosition = largePosition + new Vector3(0f, (largeSize / 2 + smallerSize / 2) * 0.7f, 0);
        Vector3 topPosition = smallerPosition + new Vector3(0f, (smallerSize / 2 + topSize / 2) * 0.8f, 0);

        // Création des sphères du corps
        CreateSphere(basePosition, baseSize, creatureColor, creatureModel);
        CreateSphere(largePosition, largeSize, creatureColor, creatureModel);
        if (creature.Type == CreatureType.Forest)
        {
            CreateSphere(smallerPosition, smallerSize, creatureColor, creatureModel);
            CreateSphere(topPosition, topSize, creatureColor, creatureModel);
        }
        else
        {
            // CreateCube(smallerPosition, baseSize, creatureColor, creatureModel);
        }

        return largePosition;  // Retourner la position de la tête pour y placer les yeux
    }

    /// <summary>
    /// Crée une sphère avec la taille et la couleur spécifiées et l'ajoute au modèle de la créature
    /// </summary>
    /// <param name="position">La position où placer la sphère</param>
    /// <param name="size">La taille de la sphère</param>
    /// <param name="color">La couleur de la sphère</param>
    /// <param name="creatureModel">Le modèle de la créature</param>
    private void CreateSphere(Vector3 position, float size, Color color, GameObject creatureModel)
    {
        GameObject bodySphere = Instantiate(spherePrefab, position, Quaternion.identity);
        bodySphere.transform.localScale = new Vector3(size, size, size);
        bodySphere.GetComponent<Renderer>().material.color = color;
        bodySphere.transform.SetParent(creatureModel.transform);
    }

    /// <summary>
    /// Crée un cube avec la taille et la couleur spécifiées et l'ajoute au modèle de la créature
    /// </summary>
    /// <param name="position">La position où placer la sphère</param>
    /// <param name="size">La taille de la sphère</param>
    /// <param name="color">La couleur de la sphère</param>
    /// <param name="creatureModel">Le modèle de la créature</param>
    private void CreateCube(Vector3 position, float size, Color color, GameObject creatureModel)
    {
        float ySize = size / 4f * 3f;
        GameObject bodySphere = Instantiate(cubePrefab, position, Quaternion.Euler(0, 45, 0));
        bodySphere.transform.localScale = new Vector3(size, ySize, size);
        bodySphere.GetComponent<Renderer>().material.color = color;
        bodySphere.transform.SetParent(creatureModel.transform);
    }

    /// <summary>
    /// Ajoute des yeux à la créature et les place sur la sphère la tête
    /// </summary>
    /// <param name="headPosition">La position de la sphère tête</param>
    /// <param name="creatureModel">Le modèle de la créature</param>
    /// <param name="creature">La créature dont les yeux doivent être générés</param>
    private void CreateEyes(Vector3 headPosition, GameObject creatureModel, Creature creature)
    {
        float scaleFactor = creature.ScaleFactor;
        float largeSize = 1.2f * scaleFactor;
        float size = 0.3f * scaleFactor;

        // Créer un oeil gauche
        Vector3 leftEyePosition = headPosition + new Vector3(
            -largeSize / 6,
            0.1f * scaleFactor,
            largeSize / 2 - 0.1f * scaleFactor);
        GameObject leftEye = Instantiate(eyePrefab, leftEyePosition, Quaternion.identity);
        leftEye.transform.localScale = new Vector3(size, size, size);
        leftEye.transform.SetParent(creatureModel.transform);

        // Créer un oeil droite
        Vector3 rightEyePosition = headPosition + new Vector3(
            largeSize / 6,
            0.1f * scaleFactor,
            largeSize / 2 - 0.1f * scaleFactor);
        GameObject rightEye = Instantiate(eyePrefab, rightEyePosition, Quaternion.identity);
        rightEye.transform.localScale = new Vector3(size, size, size);
        rightEye.transform.SetParent(creatureModel.transform);
    }

    /// <summary>
    /// Ajoute des moustaches à la créature et les place sur la sphère la tête
    /// </summary>
    /// <param name="headPosition">La position de la sphère tête</param>
    /// <param name="creatureModel">Le modèle de la créature</param>
    /// <param name="creature">La créature dont on génère les moustaches</param>
    private void CreateMoustache(Vector3 headPosition, GameObject creatureModel, Creature creature)
    {
        float scaleFactor = creature.ScaleFactor;
        float largeSize = 1.2f * scaleFactor;
        int numberOfMoustaches = creature.NumberOfMoustaches;
        float moustacheSize = 30f * scaleFactor;

        GameObject moustachePrefab = SelectMoustachePrefab(creature);

        if (numberOfMoustaches != 0)
        {
            for (int nbMoustache = 0; nbMoustache < numberOfMoustaches; nbMoustache++)
            {
                // Créer une moustache gauche
                Vector3 leftMoustachePosition = headPosition + new Vector3(
                    -largeSize / 3,
                    largeSize / 4 - 0.35f * scaleFactor - 0.08f * scaleFactor * (nbMoustache + 1),
                    largeSize / 2 - 0.1f * scaleFactor);
                GameObject leftMoustache = Instantiate(moustachePrefab, leftMoustachePosition, Quaternion.identity);
                leftMoustache.transform.localScale = new Vector3(moustacheSize, moustacheSize, moustacheSize);
                leftMoustache.transform.SetParent(creatureModel.transform);

                // Créer une moustache droite
                Vector3 rightMoustachePosition = headPosition + new Vector3(
                    largeSize / 3,
                    largeSize / 4 - 0.35f * scaleFactor - 0.08f * scaleFactor * (nbMoustache + 1),
                    largeSize / 2 - 0.1f * scaleFactor);
                GameObject rightMoustache = Instantiate(moustachePrefab, rightMoustachePosition, Quaternion.identity);
                rightMoustache.transform.rotation = Quaternion.Euler(0, 180, 0); // Rotation symétrique pour la moustache droite
                rightMoustache.transform.localScale = new Vector3(moustacheSize, moustacheSize, moustacheSize);
                rightMoustache.transform.SetParent(creatureModel.transform);
            }
        }
    }


    /// <summary>
    /// Selectionne le prefab adapté en fonction du type de moustache de la créature 
    /// </summary>
    /// <param name="creature">La créature dont ont génère les moustaches</param>
    private GameObject SelectMoustachePrefab(Creature creature)
    {
        switch (creature.TypeOfMoustache)
        {
            case 0: return moustache1Prefab;
            case 1: return moustache2Prefab;
            case 2: return moustache3Prefab;
            case 3: return moustache4Prefab;
            default: return moustache4Prefab;
        }
    }


    /// <summary>
    /// Ajoute des corne à la créature et les place sur la sphère la tête
    /// Seule les créatures du Désert ont des cornes
    /// </summary>
    /// <param name="headPosition">La position de la sphère tête</param>
    /// <param name="creatureModel">Le modèle de la créature</param>
    /// <param name="creature">La créature dont les cornes doivent être générées</param>
    private void CreateHorns(Vector3 headPosition, GameObject creatureModel, Creature creature)
    {
        float scaleFactor = creature.ScaleFactor;
        Color creatureColor = creature.Color;
        float largeSize = 1.2f * scaleFactor;
        float size = 40f * scaleFactor;

        // Créer une corne gauche
        Vector3 leftHornPosition = headPosition + new Vector3(
            0,
            largeSize / 2,
           0);
        GameObject leftHorn = Instantiate(hornPrefab, leftHornPosition, Quaternion.Euler(-90,90,0));
        leftHorn.transform.localScale = new Vector3(size, size, size);
        leftHorn.GetComponent<Renderer>().material.color = creatureColor;
        leftHorn.transform.SetParent(creatureModel.transform);

        // Créer une corne droite
        Vector3 rightHornPosition = headPosition + new Vector3(
            0,
            largeSize / 2,
            0);
        GameObject rightHorn = Instantiate(hornPrefab, rightHornPosition, Quaternion.Euler(-90,-90,0));
        rightHorn.transform.localScale = new Vector3(size, size, size);
        rightHorn.GetComponent<Renderer>().material.color = creatureColor;
        rightHorn.transform.SetParent(creatureModel.transform);
    }


    /// <summary>
    /// Génère des tentacules de sphères en fonction de la créature
    /// </summary>
    /// <param name="creatureModel">Le modèle de la créature</param>
    /// <param name="creature">La créature dont les tentacules doivent être générées</param>
    private void CreateTentacles(GameObject creatureModel, Creature creature)
    {
        int numberOfTentacles = creature.NumberOfTentacles;
        float scaleFactor = creature.ScaleFactor;
        int tentacleLength = creature.TentaclesWidth;

        for (int tentacle = 0; tentacle < numberOfTentacles; tentacle++)
        {
            Vector3 tentacleBasePosition = GetTentacleBasePosition(tentacle, numberOfTentacles, scaleFactor);
            CreateTentacleSpheres(tentacleBasePosition, creatureModel, creature, tentacleLength, scaleFactor);
        }
    }

    /// <summary>
    /// Calcule la position de base pour un tentacule donné
    /// </summary>
    /// <param name="tentacleIndex">L'index du tentacule à créer</param>
    /// <param name="numberOfTentacles">Le nombre total de tentacules</param>
    /// <param name="scaleFactor">Le facteur d'échelle de la créature</param>
    /// <returns>La position de base du tentacule</returns>
    private Vector3 GetTentacleBasePosition(int tentacleIndex, int numberOfTentacles, float scaleFactor)
    {
        // Rayon en fonction du facteur d'échelle
        float radius = 0.4f * scaleFactor;

        // Calcule l'angle en degrés pour la position du tentacule
        float angle = tentacleIndex * (360f / numberOfTentacles);
        // Convertit l'angle en radians pour le calcul trigonométrique
        float radian = angle * Mathf.Deg2Rad;

        // Calcule l'offset X et Z 
        float offsetX = radius * Mathf.Cos(radian);
        float offsetZ = radius * Mathf.Sin(radian);

        return _initialPosition + new Vector3(offsetX, 0, offsetZ) * 0.9f;
    }

    /// <summary>
    /// Crée les sphères de tentacule à partir de la position de base
    /// </summary>
    /// <param name="tentacleBasePosition">La position de base du tentacule</param>
    /// <param name="creatureModel">Le modèle de la créature</param>
    /// <param name="creature">La créature dont les tentacules doivent être générées</param>
    /// <param name="tentacleLength">La longueur du tentacule</param>
    /// <param name="scaleFactor">Le facteur d'échelle de la créature</param>
    private void CreateTentacleSpheres(Vector3 tentacleBasePosition, GameObject creatureModel, Creature creature, int tentacleLength, float scaleFactor)
    {
        Color creatureColor = creature.Color;
        CreatureType creatureType = creature.Type;
        float curveStep = 0.1f * scaleFactor;
        float downwardStep = 0.3f * scaleFactor;
        float initialSize = 0.25f * scaleFactor;
        float sizeReductionFactor = 0.05f;

        float currentSize;
        GameObject tentacleShape;
        for (int i = 0; i < tentacleLength; i++)
        {
            // Calcule la position d'offset X et Z en fonction de la courbe du génome
            float t = i * curveStep;
            Vector3 curvePosition = creature.DecodeCourbe(t, curveStep * tentacleLength);

            // Calcule la position d'offset Y pour descendre chaque sphère
            Vector3 offsetPosition = new Vector3(0, -i * downwardStep, 0);
            curvePosition += offsetPosition;

            Vector3 finalPosition = tentacleBasePosition + curvePosition;

            // Instancie la sphère de tentacule à la position calculée
            if (creatureType == CreatureType.Forest)
            {
                tentacleShape = Instantiate(spherePrefab, finalPosition, Quaternion.identity);
                currentSize = Mathf.Max(initialSize - i * sizeReductionFactor, 0.1f); // Détermine la taille de la sphère (avec 0.1 en minimum)
            }
            else
            {
                tentacleShape = Instantiate(cubePrefab, finalPosition, Quaternion.Euler(0, 45, 0));
                currentSize = Mathf.Max(initialSize * 0.8f - i * sizeReductionFactor, 0.1f); // Détermine la taille du cube (avec 0.1 en minimum)
            }
            tentacleShape.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
            tentacleShape.GetComponent<Renderer>().material.color = creatureColor;

            tentacleShape.transform.SetParent(creatureModel.transform); // attache la sphère au modèle de la créature
        }
    }

}
