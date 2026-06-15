using System;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    [SerializeField] private float patience = 30f;
    [SerializeField] private float fadeDuration;

    private Image _image;

    public float Patience => patience;
    public CustomerData CustomerData { get; private set; }
    public MenuData MenuData { get; set; }
    public float CurrentPatience { get; set; }
    
    public Action<MenuData> OnCustomerOrderChanged;
    public Action OnCustomerOutOfPatience;
    
    private void Awake()
    {
        _image = GetComponent<Image>();
        if (_image) _image.preserveAspect = true;
    }

    private void Enter()
    {
        if (_image) LMotion.Create(0f, 1f, fadeDuration).WithEase(Ease.InOutQuart).WithOnComplete(AfterFade).BindToColorA(_image);
    }

    private void AfterFade()
    {
        print($"Fade complete");
        MenuData = MenuManager.Instance?.GetRandomMenu();
        if (!MenuData) return;
        OnCustomerOrderChanged?.Invoke(MenuData);
        patience = CustomerData.patience;
        LMotion.Create(0f, patience, patience)
            .WithOnComplete(() => OnCustomerOutOfPatience?.Invoke())
            .Bind(x => CurrentPatience = x);

    }

    public void Enter(CustomerData data)
    {
        if (!data) return;
        SetData(data);
        if (_image) _image.sprite = CustomerData.customerSprite; 
        Enter();
    }

    public void SetData(CustomerData data)
    {
        CustomerData = data;
    }
}
