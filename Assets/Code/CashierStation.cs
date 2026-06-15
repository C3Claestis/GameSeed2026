using System;
using UnityEngine;

public class CashierStation : MonoBehaviour
{
    public static CashierStation Instance;

    private CustomerManager _customerManager;
    private Customer _customer;
    
    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        
        _customer = GetComponentInChildren<Customer>();
    }

    private void OnEnable()
    {
        if (_customer) _customer.OnCustomerOutOfPatience += OnCustomerOutOfPatience;
    }
    
    private void OnDisable()
    {
        if (_customer) _customer.OnCustomerOutOfPatience -= OnCustomerOutOfPatience;
    }

    private void Start()
    {
        _customerManager = CustomerManager.Instance;
    }

    [ContextMenu("Summon Customer")]
    public void SummonCustomer()
    {
        if (!_customerManager || !_customer) return;

         _customer.Enter(_customerManager.GetRandomCustomer());
    }

    private void OnCustomerOutOfPatience()
    {
        print($"Customer is out of Patience: {_customer.CustomerData}");
    }
}
