using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrepSelector : MonoBehaviour
{
    [SerializeField] private TMP_Text menuName;
    [SerializeField] private Transform taskContainer;
    [SerializeField] private GameObject taskPrefab;
    [SerializeField] private Button doneButton;

    private StationManager _stationManager;
    
    private void Start()
    {
        _stationManager = StationManager.Instance;
    }

    public void Initialize(MenuData data)
    {
        SetupList(data);
        SetupDoneButton(data);
    }

    private void SetupDoneButton(MenuData data)
    {
        if (!doneButton || !data) return;
        var result = 0;
        foreach (var task in data.recipesTask)
            if (task is { completed: true }) result++;
        doneButton.interactable = result >= data.recipesTask.Length;
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
        print($"Preping: {data}");
        if (!data) return;
        UpdateMenuName(data.menuName);
        if (!taskPrefab) return;
        foreach (var task in data.recipesTask)
        {
            var scratch = Instantiate(taskPrefab, taskContainer ?? transform);
            var image = scratch.GetComponentInChildren<Image>();
            var text = scratch.GetComponentInChildren<TMP_Text>();
            if (text) text.text = task.description;
            if (image) image.color = task.completed ? Color.green : Color.red;
        }
    }

    public void UpdateMenuName(string text)
    {
        if (!menuName) return;
        menuName.text = text;
    }

    public void HandleDone()
    {
        if (!_stationManager) return;
        _stationManager.GoToStation(_stationManager.ActiveStation,
            _stationManager.GetStation<CookingStation>().StationId);
    }
}
