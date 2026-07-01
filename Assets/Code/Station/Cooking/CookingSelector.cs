using UnityEngine;
using UnityEngine.UI;

public class CookingSelector : MonoBehaviour
{
    [SerializeField] private Text menuName;
    [SerializeField] private Transform taskContainer;
    [SerializeField] private GameObject taskPrefab;
    [SerializeField] private Button doneButton;

    [SerializeField] private IndicatorCooking indicatorCooking;

    private MenuData currentMenu;

    [Header("Alat Masak")]
    [SerializeField] private GameObject[] alatMasak;

    private ObjData objData;
    private StationManager _stationManager;

    private void Start()
    {
        _stationManager = StationManager.Instance;
        objData = GetComponent<ObjData>();
    }

    public void Initialize(MenuData data)
    {
        currentMenu = data;
        SetupList(data);
        SetupDoneButton(data);
    }

    public bool TryCompleteTask(Ingredient ingredient)
    {
        if (currentMenu == null || indicatorCooking == null)
            return false;

        foreach (var task in currentMenu.CookTask)
        {
            if (task is not CTGetIngredient cookTask)
                continue;

            if (cookTask.completed)
                continue;

            // Cek bahan
            if (cookTask.ingredient != ingredient)
                continue;

            // Cek indikator
            if ((int)cookTask.indicatorCook != indicatorCooking.CurrentIndicator)
            {
                Debug.Log("Indicator tidak sesuai.");
                continue;
            }

            // Complete task
            cookTask.completed = true;

            // Refresh UI
            SetupList(currentMenu);
            SetupDoneButton(currentMenu);

            Debug.Log("Cooking Task Complete!");

            return true;
        }

        return false;
    }

    private void SetupDoneButton(MenuData data)
    {
        if (!doneButton || !data) return;
        var result = 0;
        foreach (var task in data.CookTask)
            if (task is { completed: true }) result++;
        doneButton.interactable = result >= data.CookTask.Length;
    }

    private void SetupList(MenuData data)
    {
        if (taskContainer)
        {
            for (int i = taskContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(taskContainer.GetChild(i).gameObject);
            }
        }

        if (!data) return;

        UpdateMenuName(data.menuName);
        SpawnObject(data);

        bool isPan = data.menuName == "Nasi Goreng" || data.menuName == "Mie Goreng";

        alatMasak[0].SetActive(isPan);
        alatMasak[1].SetActive(!isPan);

        if (!taskPrefab) return;

        foreach (var task in data.CookTask)
        {
            var scratch = Instantiate(taskPrefab, taskContainer);

            Image indicatorImage = scratch.transform.GetChild(0).GetComponent<Image>();
            Image doneImage = scratch.transform.GetChild(1).GetComponent<Image>();
            Text taskText = scratch.transform.GetComponentInChildren<Text>(); ;

            // Nama task
            if (taskText)
                taskText.text = task.description;

            // Hanya CTGetIngredient yang memiliki indicator
            if (task is CTGetIngredient getIngredientTask && indicatorImage)
            {
                indicatorImage.sprite = getIngredientTask.indicator;
            }

            // Alpha done
            if (doneImage)
            {
                Color color = doneImage.color;
                color.a = task.completed ? 1f : 25f / 255f;
                doneImage.color = color;
            }
        }
    }

    private void SpawnObject(MenuData data)
    {
        ObjData.MenuType menuType;

        switch (data.menuName)
        {
            case "Nasi Goreng":
                menuType = ObjData.MenuType.NasiGoreng;
                break;

            case "Mie Goreng":
                menuType = ObjData.MenuType.MieGoreng;
                break;

            case "Steak":
                menuType = ObjData.MenuType.Steak;
                break;

            default:
                return;
        }

        var menu = objData.GetMenu(menuType);

        if (menu == null) return;

        foreach (var spawn in menu.objects)
        {
            Instantiate(spawn.prefab, spawn.point);
        }
    }

    public void UpdateMenuName(string text)
    {
        if (!menuName) return;
        menuName.text = "Task: " + text;
    }

    public void HandleDone()
    {
        if (!_stationManager) return;
        _stationManager.GoToStation(_stationManager.ActiveStation,
            _stationManager.GetStation<CookingStation>().StationId);
    }
}
