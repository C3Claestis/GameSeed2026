using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private List<CustomerData> customerData = new();
    
    public IReadOnlyList<CustomerData> CustomerData => customerData;
    
    public static CustomerManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public CustomerData GetRandomCustomer()
    {
        if (customerData.Count == 0) return null;
        var index = Random.Range(0, customerData.Count);
        return customerData[index];
    }
}