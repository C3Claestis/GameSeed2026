using System;
using UnityEngine;

public class PrepStation : MonoBehaviour, IStation
{
    [field: SerializeField] public int StationId { get; private set; }
    
    private CashierStation _cashier;
    private PrepSelector _selector;
    private ChopSystem _chop;

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
        
    }

    #endregion
    
    private void PrepareRecipe()
    {
        _chop?.SetCanvas(false);

        if (!_selector || !_cashier) return;

        var menu = _cashier.CurrentMenu;
        if (menu == null) return;

        _selector.Initialize(menu);
    }

    public void UpdatePrep()
    {
        if (!_selector || !_cashier) return;

        var menu = _cashier.CurrentMenu;
        if (menu == null) return;

        _selector.Initialize(menu);
    }

    public void Chop()
    {
        _chop?.SetCanvas(false);
    }
}
