using System;
using UnityEngine;

public class PrepStation : MonoBehaviour, IStation
{
    [field: SerializeField] public int StationId { get; private set; }
    
    private CashierStation _cashier;
    private PrepSelector _selector;
    private ChopSystem _chop;

    public ChopSystem ChopSystem => _chop;
    
    private void Awake()
    {
        _selector = GetComponentInChildren<PrepSelector>();
        _chop = GetComponentInChildren<ChopSystem>();
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

    }

    #endregion

    private void HandleActiveOrderChanged(CustomerOrder order)
    {
        if (!StationManager.Instance || StationManager.Instance.ActiveStation != StationId) return;

        UpdatePrep();
    }

    private void PrepareRecipe()
    {
        _chop?.SetCanvas(false);

        UpdatePrep();
    }

    public void UpdatePrep()
    {
        if (!_selector) return;

        var menu = _cashier ? _cashier.CurrentMenu : null;
        _selector.Initialize(menu);
    }

    public void Chop(RTCutIngredient menu)
    {
        if (!_chop) return;
        _chop.SetCanvas(true);
        _chop.Initialize(menu);
    }
}
