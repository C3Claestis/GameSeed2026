using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuSelector : MonoBehaviour
{
    [SerializeField] private float falseShakeDuration = 0.5f;
    
    [Header("References")]
    [SerializeField] private GameObject menuPrefab;
    [SerializeField] private Transform contentHolder;
    [SerializeField] private Canvas panelCanvas;
    [SerializeField] private Canvas contentCanvas;
    [SerializeField] private Button orderUpButton;
    [SerializeField] private Button menuOpenerButton;

    private CashierStation _cashierStation;
    private Customer _customer;
    private MenuManager _menuManager;
    private readonly List<Menu> _menuButtonList = new();
    private StationManager _stationManager;
    private bool _canSelect = true;

    public Menu SelectedMenu { get; private set; }
    
    private void Awake()
    {
        _cashierStation = GetComponentInParent<CashierStation>();
    }

    private void OnEnable() => Bind();

    private void OnDisable() => Unbind();

    private void Start()
    {
        _customer = _cashierStation.Customer;
        _menuManager = MenuManager.Instance;
        _stationManager = StationManager.Instance;
        Unbind();
        Bind();
        InitializeMenus();
        SetCanvasContent(false);
    }

    #region Events

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
            orderUpButton.interactable = SelectedMenu && _canSelect;
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

    #endregion
    
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
        if (target && SelectedMenu != target) SelectedMenu = target;
        else if (target && SelectedMenu == target) SelectedMenu = null;
        
        if (orderUpButton) orderUpButton.interactable = SelectedMenu && _canSelect;
        
        foreach (var menu in _menuButtonList)
        {
            if (menu != SelectedMenu)
            {
                menu.Button.image.color = Color.white;
                continue;
            }

            menu.Button.image.color = Color.yellow;
        }
    }
    
    private void HandleOrderUp()
    {
        if (!SelectedMenu || SelectedMenu.menuData != _customer.MenuData)
        {
            _canSelect = false;
            orderUpButton.interactable = _canSelect;
            orderUpButton.image.color = Color.red;
            orderUpButton.transform.DOShakePosition(falseShakeDuration, strength: 20f, vibrato: 30)
                .OnComplete(() =>
                {
                    _canSelect = true;
                    orderUpButton.interactable = SelectedMenu && _canSelect;
                    orderUpButton.image.color = Color.white;
                });
            return;
        }
        
        if (!_stationManager) return;
        SetPanelMenu(false);
        _stationManager.GoToStation(_cashierStation.StationId, _stationManager.GetStation<PrepStation>()?.StationId ?? 0);
    }

    public void SetPanelMenu(bool show)
    {
        if (contentCanvas) contentCanvas.enabled = show;
        if (menuOpenerButton) menuOpenerButton.interactable = !show;
    }
}