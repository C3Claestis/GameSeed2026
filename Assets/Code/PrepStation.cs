using System;
using UnityEngine;

public class PrepStation : MonoBehaviour, IStation
{
    [field: SerializeField] public int StationId { get; private set; }
    
    private CashierStation _cashier;
    
    private void Start()
    {
        _cashier = StationManager.Instance?.CashierStation;
    }

    public void OpenStation()
    {
        
    }
}
