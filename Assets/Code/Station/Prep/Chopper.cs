using UnityEngine;

public class Chopper : MonoBehaviour
{
    [SerializeField] private Ingredient ingredient;

    private StationManager _manager;
    private CashierStation _cashier;
    private PrepStation _prep;

    private void Start()
    {
        _manager = StationManager.Instance;
        _cashier = _manager?.GetStation<CashierStation>();
        _prep = _manager?.GetStation<PrepStation>();
    }

    public void TryChop()
    {
        if (!_cashier || !ingredient) return;
        var menu = _cashier.CurrentMenu;
        if (!menu || menu.recipesTask.Length <= 0) return;
        foreach (var task in menu.recipesTask)
        {
            if (task is not RTCutIngredient { completed: false } cut || cut.ingredient?.recipeName != ingredient.recipeName)
            {
                continue;
            }

            if (IngredientReady(ingredient, menu.recipesTask))
            {
                _prep.Chop(cut);
                break;
            }
        }
    }

    private bool IngredientReady(Ingredient target, RecipeTask[] tasks)
    {
        if (tasks == null || tasks.Length <= 0) return false;
        var i = 0;
        foreach (var task in tasks)
        {
            if (task is RTGetIngredient prepped && prepped.ingredient == target) return prepped.completed;
            i++;
        }
        
        return i == tasks.Length;
    }
}
