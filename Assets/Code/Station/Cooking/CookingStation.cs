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
        if (OrderManager.Instance != null)
            OrderManager.Instance.OnActiveOrderChanged -= HandleActiveOrderChanged;
    }

    private void Start()
    {
        _cashier = StationManager.Instance?.GetStation<CashierStation>();

        if (OrderManager.Instance != null)
            OrderManager.Instance.OnActiveOrderChanged += HandleActiveOrderChanged;
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

    private void HandleActiveOrderChanged(CustomerOrder order)
    {
        if (!StationManager.Instance || StationManager.Instance.ActiveStation != StationId) return;

        PrepareRecipe();
    }

    private void PrepareRecipe()
    {
        if (!_selector) return;

        var menu = _cashier ? _cashier.CurrentMenu : null;

        if (!IsReadyToCook(menu))
        {
            indicatorCooking?.StopIndicator();
            _selector.Initialize(null);
            return;
        }

        indicatorCooking?.StartIndicator();
        _selector.Initialize(menu);
    }

    private static bool IsReadyToCook(MenuData menu)
    {
        if (menu == null || menu.CookTask == null || menu.CookTask.Length == 0)
            return false;

        // Cek apakah semua recipesTask sudah selesai
        foreach (var task in menu.recipesTask)
        {
            if (!task.completed)
                return false;
        }

        return true;
    }

    public void UpdatePrep()
    {
        PrepareRecipe();
    }
}
