using System;
using UnityEngine;

public class PrepStation : MonoBehaviour, IStation
{
    [field: SerializeField] public int StationId { get; private set; }
    
    private CashierStation _cashier;
    private PrepSelector _selector;

    private void Awake()
    {
        _selector = GetComponentInChildren<PrepSelector>();
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
        if (!_selector || !_cashier) return;

        var menu = _cashier.CurrentMenu;
        if (menu == null) return;

        _selector.Initialize(menu);
    }
}
