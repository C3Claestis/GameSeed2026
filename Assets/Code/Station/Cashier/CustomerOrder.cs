using UnityEngine;

[System.Serializable]
public class CustomerOrder
{
    public CustomerData Customer;
    public MenuData Menu;

    public bool IsAccepted;
    public bool IsPrepFinished;
    public bool IsCookFinished;

    public CustomerOrder(CustomerData customer, MenuData menu)
    {
        Customer = customer;
        Menu = menu;
    }
}