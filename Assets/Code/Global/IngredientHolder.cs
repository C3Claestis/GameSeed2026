using UnityEngine;

public class IngredientHolder : MonoBehaviour
{
    [SerializeField] private Ingredient ingredient;

    public Ingredient Ingredient => ingredient;
}
