using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;

    [ReadOnly] private List<CustomerOrder> orders = new();

    public IReadOnlyList<CustomerOrder> Orders => orders;
    public CustomerOrder ActiveOrder { get; private set; }

    [ShowNativeProperty]
    private string ActiveOrderDebug =>
        ActiveOrder != null
            ? $"{ActiveOrder.Customer?.customerName} -> {ActiveOrder.Menu?.menuName}"
            : "(none)";

    public event Action OnOrdersChanged;
    public event Action<CustomerOrder> OnActiveOrderChanged;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public CustomerOrder AddOrder(CustomerData customer, MenuData menuTemplate)
    {
        if (!menuTemplate)
            return null;

        var order = new CustomerOrder(customer, Instantiate(menuTemplate));
        orders.Add(order);

        OnOrdersChanged?.Invoke();
        SetActiveOrder(order);

        return order;
    }

    public void SetActiveOrder(CustomerOrder order)
    {
        if (order != null && !orders.Contains(order))
            return;

        ActiveOrder = order;
        OnActiveOrderChanged?.Invoke(order);
    }

    public void RemoveOrder(CustomerOrder order)
    {
        if (order == null || !orders.Remove(order))
            return;

        OnOrdersChanged?.Invoke();

        if (ActiveOrder == order)
        {
            SetActiveOrder(orders.Count > 0 ? orders[0] : null);
        }
    }

    [ContextMenu("Cycle Active Order"), Button("Cycle Active Order", enabledMode: EButtonEnableMode.Playmode)]
    private void CycleActiveOrder()
    {
        if (orders.Count == 0) return;

        var index = ActiveOrder != null ? orders.IndexOf(ActiveOrder) : -1;
        var next = orders[(index + 1) % orders.Count];
        SetActiveOrder(next);
    }
}
