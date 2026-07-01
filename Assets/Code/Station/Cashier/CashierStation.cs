using System;
using NaughtyAttributes;
using UnityEngine;

public class CashierStation : MonoBehaviour, IStation
{
    public static CashierStation Instance;

    [field: SerializeField] public int StationId { get; private set; }

    private CustomerManager _customerManager;

    public Customer Customer { get; private set; }
    public MenuData CurrentMenu => OrderManager.Instance?.ActiveOrder?.Menu;

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

    #region Station Manipulation

    public void OnOpen()
    {
        
    }

    public void OnClose()
    {
        
    }

    #endregion

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