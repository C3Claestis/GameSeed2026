using System;
using UnityEngine;

public class IngredientCook : MonoBehaviour
{
    [SerializeField] private Ingredient ingredient;

    public Ingredient Ingredient => ingredient;

    private CashierStation _cashier;
    private PrepStation _prep;
    private CookingSelector _cookingSelector;

    private void Start()
    {
        var manager = StationManager.Instance;
        if (!manager) return;

        _cashier = manager.GetStation<CashierStation>();
        _prep = manager.GetStation<PrepStation>();

        _cookingSelector = FindFirstObjectByType<CookingSelector>();
    }

    public void SubmitIngredient()
    {
        if (!_cashier || !_cashier.CurrentMenu)
            return;

        // ===== Prep Task =====
        foreach (var task in _cashier.CurrentMenu.recipesTask)
        {
            if (task is RTGetIngredient rtgi &&
                !rtgi.completed &&
                rtgi.ingredient == Ingredient)
            {
                rtgi.completed = true;
                _prep?.UpdatePrep();
            }
        }

        // ===== Cooking Task =====
        _cookingSelector?.TryCompleteTask(Ingredient);
    }
}