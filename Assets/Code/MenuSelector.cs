using UnityEngine;

public class MenuSelector : MonoBehaviour
{
    [SerializeField] private GameObject menuPrefab;
    [SerializeField] private Transform contentHolder;
    [SerializeField] private Canvas contentCanvas;

    private CashierStation _cashierStation;
    private Customer _customer;
    private MenuManager _menuManager;
    
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
        SetCanvas(false);
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
    }
    
    private void OnNewCustomerEnter()
    {
        SetCanvas(false);
    }
    
    private void OnCustomerOrderChanged(MenuData obj)
    {
        print($"OnCustomerOrderChanged: {obj} | {menuPrefab} | {_menuManager}");
        SetCanvas(true);
    }

    private void SetCanvas(bool value)
    {
        if (!contentCanvas) return;
        contentCanvas.enabled = value;
    }

    private void InitializeMenus()
    {
        if (!menuPrefab) return;
        foreach (var data in _menuManager.MenuData)
        {
            var menuGo = contentHolder ? Instantiate(menuPrefab, contentHolder) : Instantiate(menuPrefab, transform);
            if (menuGo.TryGetComponent(out Menu menu))
            {
                menu.menuData = data;
                menu.SetText();
            }
        }
    }
}