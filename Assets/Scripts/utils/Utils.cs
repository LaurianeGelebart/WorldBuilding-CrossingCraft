using UnityEngine;

/// <summary>
/// Classe utilitaire contenant des méthodes statiques génériques
/// </summary>
public static class Utils 
{
    /// <summary>
    /// Combine deux bits en un entier unique
    /// </summary>
    /// <param name="bit1">Premier bit à combiner</param>
    /// <param name="bit2">Deuxième bit à combiner</param>
    /// <returns>Entier résultant de la combinaison des bits</returns>
    public static int BitToInt(int bit1, int bit2)
    {
        return (bit1 << 1) | bit2; // Combine les deux bits en un entier
    }

    /// <summary>
    /// Retourne la couleur Navajo White (blanc cassé)
    /// </summary>
    public static Color GetNavajoWhite()
    {
        return new Color(255f / 255f, 222f / 255f, 173f / 255f);
    }

    /// <summary>
    /// Retourne la couleur Light Slate Gray (gris ardoise clair)
    /// </summary>
    public static Color GetLightSlateGray()
    {
        return new Color(119f / 255f, 136f / 255f, 153f / 255f);
    }

    /// <summary>
    /// Retourne la couleur Light Sky Blue (bleu ciel clair)
    /// </summary>
    public static Color GetLightSkyBlue()
    {
        return new Color(135f / 255f, 206f / 255f, 250f / 255f);
    }

    /// <summary>
    /// Retourne la couleur Medium Sea Green (vert marin moyen)
    /// </summary>
    public static Color GetMediumSeaGreen()
    {
        return new Color(60f / 255f, 179f / 255f, 113f / 255f);
    }
}