using System;
using UnityEngine;

public class IngredientHolder : MonoBehaviour
{
    [SerializeField] private Ingredient ingredient;

    public Ingredient Ingredient => ingredient;

    private CashierStation _cashier;
    private PrepStation _prep;
    
    private void Start()
    {
        var manager = StationManager.Instance;
        if (!manager) return;
        _cashier = manager.GetStation<CashierStation>();
        _prep = manager.GetStation<PrepStation>();
    }

    public void SubmitIngredient()
    {
        if (!_cashier || !_cashier.CurrentMenu || _cashier.CurrentMenu.recipesTask.Length <= 0) return;
        foreach (var task in _cashier.CurrentMenu.recipesTask)
        {
            if (task is RTGetIngredient rtgi && rtgi.ingredient == Ingredient) rtgi.completed = true;
        }
    }
}
