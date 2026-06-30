using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuSelector : MonoBehaviour
{
    [SerializeField] private float falseShakeDuration = 0.5f;

    [Header("References")]
    [SerializeField] private Canvas panelCanvas;
    [SerializeField] private Canvas contentCanvas;
    [SerializeField] private Button orderUpButton;
    [SerializeField] private Button menuOpenerButton;

    [Header("Menu Buttons")]
    [SerializeField] private Button[] menuButtons;     // Isi 3 Button
    [SerializeField] private Text[] menuTexts;     // Isi 3 TMP_Text

    private CashierStation _cashierStation;
    private Customer _customer;
    private MenuManager _menuManager;
    private StationManager _stationManager;

    private bool _canSelect = true;
    private MenuData _selectedMenu;

    public MenuData CurrentMenu { get; private set; }

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
            orderUpButton.onClick.RemoveListener(HandleOrderUp);
            orderUpButton.onClick.AddListener(HandleOrderUp);
            orderUpButton.interactable = _selectedMenu != null && _canSelect;
        }
    }

    private void Unbind()
    {
        if (_cashierStation)
            _cashierStation.OnNewCustomerEnter -= OnNewCustomerEnter;

        if (_customer)
            _customer.OnCustomerOrderChanged -= OnCustomerOrderChanged;

        if (orderUpButton)
            orderUpButton.onClick.RemoveListener(HandleOrderUp);
    }

    #endregion

    #region Canvas

    private void SetCanvasContent(bool value)
    {
        if (contentCanvas)
            contentCanvas.enabled = value;
    }

    private void SetCanvasPanel(bool value)
    {
        if (panelCanvas)
            panelCanvas.enabled = value;
    }

    #endregion

    private void OnNewCustomerEnter()
    {
        SetCanvasContent(false);
    }

    private void OnCustomerOrderChanged(MenuData obj)
    {
        Debug.Log($"Customer Order : {obj.name}");
    }

    private void InitializeMenus()
    {
        if (_menuManager == null)
            return;

        for (int i = 0; i < menuButtons.Length; i++)
        {
            if (i >= _menuManager.MenuData.Count)
            {
                menuButtons[i].gameObject.SetActive(false);
                continue;
            }

            var data = _menuManager.MenuData[i];

            menuButtons[i].gameObject.SetActive(true);

            // Ganti MenuName sesuai field pada MenuData milikmu
            menuTexts[i].text = data.menuName;

            int index = i;

            menuButtons[i].onClick.RemoveAllListeners();
            menuButtons[i].onClick.AddListener(() => HandleMenuButtonClicked(index));

            menuTexts[i].color = Color.black;
        }
    }

    private void HandleMenuButtonClicked(int index)
    {
        if (index >= _menuManager.MenuData.Count)
            return;

        if (_selectedMenu == _menuManager.MenuData[index])
            _selectedMenu = null;
        else
            _selectedMenu = _menuManager.MenuData[index];

        if (orderUpButton)
            orderUpButton.interactable = _selectedMenu != null && _canSelect;

        RefreshButtonColors();
    }

    private void RefreshButtonColors()
    {
        for (int i = 0; i < menuTexts.Length; i++)
        {
            if (i >= _menuManager.MenuData.Count)
                continue;

            menuTexts[i].color =
                _selectedMenu == _menuManager.MenuData[i]
                ? Color.green
                : Color.black;
        }
    }

    private void HandleOrderUp()
    {
        if (_selectedMenu == null ||
            !_customer ||
            _customer.MenuSource == null ||
            _selectedMenu != _customer.MenuSource)
        {
            _canSelect = false;

            orderUpButton.interactable = false;
            orderUpButton.image.color = Color.red;

            orderUpButton.transform
                .DOShakePosition(falseShakeDuration, strength: 20f, vibrato: 30)
                .OnComplete(() =>
                {
                    _canSelect = true;
                    orderUpButton.interactable = _selectedMenu != null;
                    orderUpButton.image.color = Color.white;
                });

            return;
        }

        if (!CurrentMenu || CurrentMenu != _selectedMenu)
        {
            CurrentMenu = Instantiate(_selectedMenu);
        }

        SetPanelMenu(false);

        if (_stationManager)
        {
            _stationManager.GoToStation(
                _cashierStation.StationId,
                _stationManager.GetStation<PrepStation>()?.StationId ?? 0);
        }
    }

    public void SetPanelMenu(bool show)
    {
        if (contentCanvas)
            contentCanvas.enabled = show;

        if (menuOpenerButton)
            menuOpenerButton.interactable = !show;
    }
}