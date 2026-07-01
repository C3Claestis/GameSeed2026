using System;
using UnityEngine;

public class CookingStation : MonoBehaviour, IStation
{
    [field: SerializeField] public int StationId { get; private set; }

    [SerializeField] private IndicatorCooking indicatorCooking;

    private CashierStation _cashier;
    private CookingSelector _selector;

    private void Awake()
    {
        _selector = GetComponentInChildren<CookingSelector>();
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void Start()
    {
        _cashier = StationManager.Instance?.GetStation<CashierStation>();
    }

    #region Station Manipulation

    public void OnOpen()
    {
        PrepareRecipe();
    }

    public void OnClose()
    {
        indicatorCooking?.StopIndicator();
    }

    #endregion

    private void PrepareRecipe()
    {
        if (!_selector || !_cashier) return;

        var menu = _cashier.CurrentMenu;
        if (menu == null || menu.CookTask == null || menu.CookTask.Length == 0)
            return;

        // Cek apakah semua recipesTask sudah selesai
        foreach (var task in menu.recipesTask)
        {
            if (!task.completed)
                return;
        }

        indicatorCooking?.StartIndicator();
        _selector.Initialize(menu);
    }

    public void UpdatePrep()
    {
        if (!_selector || !_cashier) return;

        var menu = _cashier.CurrentMenu;
        if (menu == null) return;

        UpdateSelector(menu);
    }

    private void UpdateSelector(MenuData menu)
    {
        _selector.Initialize(menu);
    }
}
