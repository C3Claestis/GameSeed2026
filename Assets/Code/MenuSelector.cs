using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuSelector : MonoBehaviour
{
    [SerializeField] private GameObject menuPrefab;
    [SerializeField] private Transform contentHolder;
    [SerializeField] private Canvas panelCanvas;
    [SerializeField] private Canvas contentCanvas;
    [SerializeField] private Button orderUpButton;

    private CashierStation _cashierStation;
    private Customer _customer;
    private MenuManager _menuManager;
    private readonly List<Menu> _menuButtonList = new();
    private Menu _selectedMenu;
    
    private void Awake()
    {
        _cashierStation = GetComponentInParent<CashierStation>();
    }

    private void OnEnable()
    {
        Bind();
    }

    private void OnDisable()
    {
        Unbind();
    }

    private void Start()
    {
        _customer = _cashierStation.Customer;
        _menuManager = MenuManager.Instance;
        Unbind();
        Bind();
        InitializeMenus();
        SetCanvasContent(false);
    }

    private void Bind()
    {
        if (_cashierStation)
        {
            _cashierStation.OnNewCustomerEnter -= OnNewCustomerEnter;
            _cashierStation.OnNewCustomerEnter += OnNewCustomerEnter;
        }
        if (_customer)
        {
            _customer.OnCustomerOrderChanged -= OnCustomerOrderChanged;
            _customer.OnCustomerOrderChanged += OnCustomerOrderChanged;
        }
        if (orderUpButton)
        {
            orderUpButton.onClick.AddListener(HandleOrderUp);
            orderUpButton.interactable = _selectedMenu;
        }
    }
    
    private void Unbind()
    {
        if (_cashierStation)
        {
            _cashierStation.OnNewCustomerEnter -= OnNewCustomerEnter;
        }
        if (_customer)
        {
            _customer.OnCustomerOrderChanged -= OnCustomerOrderChanged;
        }
        if (orderUpButton)
        {
            orderUpButton.onClick.RemoveListener(HandleOrderUp);
        }

    }
    
    #region Canvas Manipulation

    private void SetCanvasContent(bool value)
    {
        if (!contentCanvas) return;
        contentCanvas.enabled = value;
    }

    private void SetCanvasPanel(bool value)
    {
        if (!panelCanvas) return;
        panelCanvas.enabled = value;
    }

    #endregion
    
    private void OnNewCustomerEnter()
    {
        SetCanvasContent(false);
    }
    
    private void OnCustomerOrderChanged(MenuData obj)
    {
        print($"OnCustomerOrderChanged: {obj} | {menuPrefab} | {_menuManager}");
    }
    
    private void InitializeMenus()
    {
        if (!menuPrefab) return;
        foreach (var data in _menuManager.MenuData)
        {
            var menuGo = contentHolder ? Instantiate(menuPrefab, contentHolder) : Instantiate(menuPrefab, transform);
            if (menuGo.TryGetComponent(out Menu menu))
            {
                _menuButtonList.Add(menu);
                menu.menuData = data;
                menu.SetText();
                menu.OnSelected += HandleMenuButtonClicked;
            }
        }
    }

    private void HandleMenuButtonClicked(Menu target)
    {
        if (target && _selectedMenu != target) _selectedMenu = target;
        else if (target && _selectedMenu == target) _selectedMenu = null;
        
        if (orderUpButton) orderUpButton.interactable = _selectedMenu;
        
        foreach (var menu in _menuButtonList)
        {
            if (menu != _selectedMenu)
            {
                menu.Button.image.color = Color.white;
                continue;
            }

            menu.Button.image.color = Color.yellow;
        }
    }
    
    private void HandleOrderUp()
    {
        if (!_selectedMenu || _selectedMenu.menuData != _customer.MenuData)
        {
            orderUpButton.interactable = false;
            orderUpButton.image.color = Color.red;
            orderUpButton.transform.DOShakePosition(0.8f, strength: 20f, vibrato: 30)
                .OnComplete(() =>
                {
                    orderUpButton.interactable = true;
                    orderUpButton.image.color = Color.white;
                });
        }
    }
}