using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestCreatureGenerator : CreatureGenerator<ForestCreature> 
{
    public GameObject eyePrefab;    // Prefab pour les yeux 
    public GameObject spherePrefab; // Prefab pour les sphères du corps

    private Vector3 initialPosition;
    

    /// <summary>
    /// Génère un modèle 3D à partir du génome d'une créature
    /// </summary>
    /// <param name="creature">La créature dont le modèle doit être généré</param>
    /// <returns>Le modèle GameObject généré</returns>
    public override GameObject GenerateModel(ForestCreature creature)
    {
        GameObject creatureModel = new GameObject("CreatureModel");
        this.initialPosition = new Vector3(Random.Range(0, 50), Random.Range(0, 10), Random.Range(0, 50));
        creatureModel.transform.position = initialPosition;

        Vector3 headPosition = CreateBody(spherePrefab, initialPosition, creatureModel, creature);
        CreateEyes(eyePrefab, headPosition, creatureModel, creature);
        CreateTentacles(spherePrefab, initialPosition, creatureModel, creature);

        return creatureModel;
    }

    /// <summary>
    /// Génère les sphères du corps et retourne la position de la dernière sphère (tête)
    /// </summary>
    /// <param name="spherePrefab">Le prefab de la sphère</param>
    /// <param name="initialPosition">La position initiale</param>
    /// <param name="creatureModel">Le modèle de créature à faire évoluer</param>
    /// <param name="creature">La créature dont le modèle est en cours de création</param>
    /// <returns>La position de la dernière sphère (tête)</returns>
    private Vector3 CreateBody(GameObject spherePrefab, Vector3 initialPosition, GameObject creatureModel, ForestCreature creature)
    {
        float scaleFactor = creature.ScaleFactor;
        Color creatureColor = creature.Color;

        // Tailles des sphères du corps basées sur le scaleFactor contenu dans le genome
        float baseSize = 0.5f * scaleFactor;
        float largeSize = 1.2f * scaleFactor;
        float smallerSize = 0.7f * scaleFactor;
        float topSize = 0.3f * scaleFactor;

        // Positionement de chaque sphère
        Vector3 basePosition = initialPosition;
        Vector3 largePosition = basePosition + new Vector3(0, (baseSize / 2 + largeSize / 2) * 0.8f, 0);
        Vector3 smallerPosition = largePosition + new Vector3(0.2f, (largeSize / 2 + smallerSize / 2) * 0.8f, 0);
        Vector3 topPosition = smallerPosition + new Vector3(0.2f, (smallerSize / 2 + topSize / 2) * 0.8f, 0);

        // Création des sphères du corps
        CreateSphere(spherePrefab, basePosition, baseSize, creatureColor, creatureModel);
        CreateSphere(spherePrefab, largePosition, largeSize, creatureColor, creatureModel);
        CreateSphere(spherePrefab, smallerPosition, smallerSize, creatureColor, creatureModel);
        CreateSphere(spherePrefab, topPosition, topSize, creatureColor, creatureModel);

        return largePosition;  // Retourner la position de la tête pour y placer les yeux
    }

    /// <summary>
    /// Crée une sphère avec la taille et la couleur spécifiées et l'ajoute au modèle de la créature
    /// </summary>
    /// <param name="spherePrefab">Le prefab de la sphère</param>
    /// <param name="position">La position où placer la sphère</param>
    /// <param name="size">La taille de la sphère</param>
    /// <param name="color">La couleur de la sphère</param>
    /// <param name="creatureModel">Le modèle de la créature</param>
    private void CreateSphere(GameObject spherePrefab, Vector3 position, float size, Color color, GameObject creatureModel)
    {
        GameObject bodySphere = Instantiate(spherePrefab, position, Quaternion.identity);
        bodySphere.transform.localScale = new Vector3(size, size, size);
        bodySphere.GetComponent<Renderer>().material.color = color;
        bodySphere.transform.SetParent(creatureModel.transform);
    }

    /// <summary>
    /// Ajoute des yeux à la créature et les place sur la sphère la tête
    /// </summary>
    /// <param name="eyePrefab">Le prefab de l'oeil</param>
    /// <param name="headPosition">La position de la sphère tête</param>
    /// <param name="creatureModel">Le modèle de la créature</param>
    /// <param name="creature">La créature dont les yeux doivent être générés</param>
    private void CreateEyes(GameObject eyePrefab, Vector3 headPosition, GameObject creatureModel, ForestCreature creature)
    {
        float scaleFactor = creature.ScaleFactor;
        float largeSize = 1.2f * scaleFactor;
        float size = 0.3f * scaleFactor;

        // Créer un oeil gauche
        Vector3 leftEyePosition = headPosition + new Vector3(-largeSize / 6, 0.1f * scaleFactor, largeSize / 2 - 0.1f * scaleFactor);
        GameObject leftEye = Instantiate(eyePrefab, leftEyePosition, Quaternion.identity);
        leftEye.transform.localScale = new Vector3(size, size, size);
        leftEye.transform.SetParent(creatureModel.transform);

        // Créer un oeil droite
        Vector3 rightEyePosition = headPosition + new Vector3(largeSize / 6, 0.1f * scaleFactor, largeSize / 2 - 0.1f * scaleFactor);
        GameObject rightEye = Instantiate(eyePrefab, rightEyePosition, Quaternion.identity);
        rightEye.transform.localScale = new Vector3(size, size, size);
        rightEye.transform.SetParent(creatureModel.transform);
    }


    /// <summary>
    /// Génère des tentacules de sphères en fonction de la créature
    /// </summary>
    /// <param name="spherePrefab">Le prefab de la sphère</param>
    /// <param name="initialPosition">La position initiale des tentacules</param>
    /// <param name="creatureModel">Le modèle de la créature</param>
    /// <param name="creature">La créature dont les tentacules doivent être générées</param>
    private void CreateTentacles(GameObject spherePrefab, Vector3 initialPosition, GameObject creatureModel, ForestCreature creature)
    {
        int numberOfTentacles = creature.NumberOfTentacles;
        float scaleFactor = creature.ScaleFactor;
        int tentacleLength = creature.TentaclesWidth;

        for (int tentacle = 0; tentacle < numberOfTentacles; tentacle++)
        {
            Vector3 tentacleBasePosition = GetTentacleBasePosition(initialPosition, tentacle, numberOfTentacles, scaleFactor);
            CreateTentacleSpheres(spherePrefab, tentacleBasePosition, creatureModel, creature, tentacleLength, scaleFactor);
        }
    }

    /// <summary>
    /// Calcule la position de base pour un tentacule donné
    /// </summary>
    /// <param name="initialPosition">La position initiale des tentacules</param>
    /// <param name="tentacleIndex">L'index du tentacule à créer</param>
    /// <param name="numberOfTentacles">Le nombre total de tentacules</param>
    /// <param name="scaleFactor">Le facteur d'échelle de la créature</param>
    /// <returns>La position de base du tentacule</returns>
    private Vector3 GetTentacleBasePosition(Vector3 initialPosition, int tentacleIndex, int numberOfTentacles, float scaleFactor)
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

        return initialPosition + new Vector3(offsetX, 0, offsetZ) * 0.9f;
    }

    /// <summary>
    /// Crée les sphères de tentacule à partir de la position de base
    /// </summary>
    /// <param name="spherePrefab">Le prefab de la sphère</param>
    /// <param name="tentacleBasePosition">La position de base du tentacule</param>
    /// <param name="creatureModel">Le modèle de la créature</param>
    /// <param name="creature">La créature dont les tentacules doivent être générées</param>
    /// <param name="tentacleLength">La longueur du tentacule</param>
    /// <param name="scaleFactor">Le facteur d'échelle de la créature</param>
    private void CreateTentacleSpheres(GameObject spherePrefab, Vector3 tentacleBasePosition, GameObject creatureModel, ForestCreature creature, int tentacleLength, float scaleFactor)
    {
        Color creatureColor = creature.Color;
        float curveStep = 0.1f * scaleFactor;
        float downwardStep = 0.3f * scaleFactor;
        float initialSphereSize = 0.25f * scaleFactor;
        float sizeReductionFactor = 0.05f;

        for (int i = 0; i < tentacleLength; i++)
        {
            // Calcule la position d'offset X et Z en fonction de la courbe du génome
             float t = i * curveStep;
            Vector3 curvePosition = creature.DecodeCourbe(t, curveStep*tentacleLength);

            // Calcule la position d'offset Y pour descendre chaque sphère
            Vector3 offsetPosition = new Vector3(0, -i * downwardStep, 0);
            curvePosition += offsetPosition; 

            Vector3 finalPosition = tentacleBasePosition + curvePosition;
            float currentSphereSize = Mathf.Max(initialSphereSize - i * sizeReductionFactor, 0.1f); // Détermine la taille de la sphère (avec 0.1 en minimum)

            // Instancie la sphère de tentacule à la position calculée
            GameObject tentacleSphere = Instantiate(spherePrefab, finalPosition, Quaternion.identity);
            tentacleSphere.transform.localScale = new Vector3(currentSphereSize, currentSphereSize, currentSphereSize);
            tentacleSphere.GetComponent<Renderer>().material.color = creatureColor; 

            tentacleSphere.transform.SetParent(creatureModel.transform); // attache la sphère au modèle de la créature
        }
    }

}
