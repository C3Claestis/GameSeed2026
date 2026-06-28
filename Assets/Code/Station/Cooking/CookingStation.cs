using System;
using UnityEngine;

public class CookingStation : MonoBehaviour, IStation
{
    [field: SerializeField] public int StationId { get; private set; }

    private CashierStation _cashier;
    
    private void Start()
    {
        _cashier = StationManager.Instance?.GetStation<CashierStation>();
    }

    #region Station Manipulation

    public void OnOpen()
    {
        
    }

    public void OnClose()
    {
        
    }

    #endregion
}