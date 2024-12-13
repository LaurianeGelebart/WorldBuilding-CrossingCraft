using UnityEngine;

public enum FoodCategory
{
    Carrot,
    Cauliflower,
    Pear,
    Pineapple,
    Tomato,
    Mushroom
}

public class FoodItem : MonoBehaviour
{
    public FoodCategory foodCategory = FoodCategory.Carrot;
    public float nutritionalValue = 50f;  // Valeur nutritive standard
    public float poisonIntensity = 0f;    // Intensit� du poison pour les champignons
}
