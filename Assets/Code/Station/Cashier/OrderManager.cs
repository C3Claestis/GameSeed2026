using System;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;

    private readonly Queue<MenuData> waitingOrders = new();

    public event Action OnQueueChanged;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Tambahkan menu baru ke antrean.
    /// </summary>
    public void AddOrder(MenuData menu)
    {
        if (menu == null)
            return;

        // Instantiate agar progress task tiap order tidak saling berbagi
        waitingOrders.Enqueue(Instantiate(menu));

        OnQueueChanged?.Invoke();
    }

    /// <summary>
    /// Melihat menu pertama tanpa menghapus.
    /// </summary>
    public MenuData GetCurrentOrder()
    {
        return waitingOrders.Count > 0
            ? waitingOrders.Peek()
            : null;
    }

    /// <summary>
    /// Menghapus menu pertama setelah selesai.
    /// </summary>
    public MenuData CompleteCurrentOrder()
    {
        if (waitingOrders.Count == 0)
            return null;

        MenuData menu = waitingOrders.Dequeue();

        OnQueueChanged?.Invoke();

        return menu;
    }

    public bool HasOrder => waitingOrders.Count > 0;

    public int Count => waitingOrders.Count;

    public IReadOnlyCollection<MenuData> Orders => waitingOrders;
}