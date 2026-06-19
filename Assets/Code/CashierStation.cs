using System;
using NaughtyAttributes;
using UnityEngine;

public class CashierStation : MonoBehaviour
{
    public static CashierStation Instance;

    private CustomerManager _customerManager;
    public Customer Customer { get; private set; }

    public Action OnNewCustomerEnter;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        
        Customer = GetComponentInChildren<Customer>();
    }

    private void OnEnable()
    {
        if (Customer) Customer.OnCustomerOutOfPatience += OnCustomerOutOfPatience;
    }
    
    private void OnDisable()
    {
        if (Customer) Customer.OnCustomerOutOfPatience -= OnCustomerOutOfPatience;
    }

    private void Start()
    {
        _customerManager = CustomerManager.Instance;
        SummonCustomer();
    }

    [ContextMenu("Summon Customer"), Button(enabledMode: EButtonEnableMode.Playmode)]
    public void SummonCustomer()
    {
        if (!_customerManager || !Customer) return;

         Customer.Enter(_customerManager.GetRandomCustomer());
         OnNewCustomerEnter?.Invoke();
    }

    private void OnCustomerOutOfPatience()
    {
        print($"Customer is out of Patience: {Customer.CustomerData}");
    }
}
