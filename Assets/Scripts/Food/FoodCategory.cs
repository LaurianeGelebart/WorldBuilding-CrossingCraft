using UnityEngine;

public enum FoodCategory
{
    Standard,
    Mushroom
}

public class FoodItem : MonoBehaviour
{
    public FoodCategory foodCategory = FoodCategory.Standard;
    public float nutritionalValue = 50f;  // Valeur nutritive standard
    public float poisonIntensity = 0f;    // Intensité du poison pour les champignons
}
