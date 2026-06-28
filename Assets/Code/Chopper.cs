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
        print($"Try chop {ingredient}");
        if (!_cashier || !ingredient) return;
        var menu = _cashier.CurrentMenu;
        if (!menu || menu.recipesTask.Length <= 0) return;
        foreach (var task in menu.recipesTask)
        {
            print($"Apakah sama: {ingredient.recipeName} | {(task as RTCutIngredient)?.ingredient?.recipeName} | {(task as RTCutIngredient)?.ingredient?.recipeName != ingredient.recipeName}");
            if (task is not RTCutIngredient { completed: false } cut || cut.ingredient?.recipeName != ingredient.recipeName)
            {
                continue;
            }
            _prep.Chop(cut);
            break;
        }
    }
}
