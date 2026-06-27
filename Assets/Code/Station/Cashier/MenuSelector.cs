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
    private Menu _selectedMenu;
    private StationManager _stationManager;
    private bool _canSelect = true;
    
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
        _stationManager = StationManager.Instance;
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
            orderUpButton.interactable = _selectedMenu && _canSelect;
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
        
        if (orderUpButton) orderUpButton.interactable = _selectedMenu && _canSelect;
        
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
            _canSelect = false;
            orderUpButton.interactable = _canSelect;
            orderUpButton.image.color = Color.red;
            orderUpButton.transform.DOShakePosition(falseShakeDuration, strength: 20f, vibrato: 30)
                .OnComplete(() =>
                {
                    _canSelect = true;
                    orderUpButton.interactable = _selectedMenu && _canSelect;
                    orderUpButton.image.color = Color.white;
                });
            return;
        }
        
        if (!_stationManager) return;
        SetPanelMenu(false);
        _stationManager.GoToStation(_cashierStation.StationId, _stationManager.PrepStation.StationId);
    }

    public void SetPanelMenu(bool show)
    {
        if (contentCanvas) contentCanvas.enabled = show;
        if (menuOpenerButton) menuOpenerButton.interactable = !show;
    }
}