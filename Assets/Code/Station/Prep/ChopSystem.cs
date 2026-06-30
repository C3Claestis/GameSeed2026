using System;
using UnityEngine;
using UnityEngine.UI;

public class ChopSystem : MonoBehaviour
{
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private GameObject ingredientPrefabGarlic;
    [SerializeField] private GameObject knifePrefab;
    [SerializeField] private GameObject knifePrefabGarlic;
    [SerializeField] private CanvasGroup prepCanvas;
    [SerializeField] private CanvasGroup choppingCanvas;
    [SerializeField] private Transform cuttingBoard;

    private PrepStation _prep;
    private Image _target;
    private Button _knife;

    public RTCutIngredient Cut { get; private set; }

    private void Start()
    {
        _prep = StationManager.Instance?.GetStation<PrepStation>();
    }

    public void SetCanvas(bool value)
    {
        if (prepCanvas)
        {
            prepCanvas.interactable = !value;
            prepCanvas.blocksRaycasts = !value;
            prepCanvas.alpha = !value ? 1f : 0;
        }
        if (choppingCanvas)
        {
            choppingCanvas.interactable = value;
            choppingCanvas.blocksRaycasts = value;
            choppingCanvas.alpha = value ? 1f : 0;
        }
    }

    public void Initialize(RTCutIngredient menu)
    {
        if (cuttingBoard)
        {
            for (int i = cuttingBoard.childCount - 1; i >= 0; i--)
            {
                Destroy(cuttingBoard.GetChild(i).gameObject);
            }
        }

        if (menu == null) return;

        //Tambahan Jafar
        GameObject ingredientToSpawn;
        GameObject knifeToSpawn;

        switch (menu.knifeType)
        {
            case RTCutIngredient.KnifeType.Garlic:
                ingredientToSpawn = ingredientPrefabGarlic;
                knifeToSpawn = knifePrefabGarlic;
                break;

            default:
                ingredientToSpawn = ingredientPrefab;
                knifeToSpawn = knifePrefab;
                break;
        }

        var targetGo = Instantiate(ingredientToSpawn, cuttingBoard);
        _target = targetGo.GetComponent<Image>();

        _knife = Instantiate(knifeToSpawn, targetGo.transform).GetComponent<Button>();
        //Tambahan Jafar

        _target.sprite = menu.cutSteps[0]?.sprite;
        Cut = menu;
        var knifePos = new Vector3(Cut.cutSteps[0]?.knifePosition.x ?? 0, -110, Cut.cutSteps[0]?.knifePosition.y ?? 0);
        _knife.transform.localPosition = knifePos;
    }

    public void UpdateSteps()
    {
        if (Cut == null) return;
        var stepComplete = 0;
        var steps = Cut.cutSteps;
        for (var i = 0; i < Cut.cutSteps.Count; i++)
        {
            var step = Cut.cutSteps[i];
            if (step.completed)
            {
                stepComplete++;
                continue;
            }

            if (i < steps.Count - 1)
            {
                if (_target) _target.sprite = steps[i + 1]?.sprite;
                if (_knife) _knife.transform.localPosition = steps[i + 1]?.knifePosition ?? Vector3.zero;
            }
            else
            {
                if (_target) _target.sprite = Cut.resultSprite;
                if (_knife) _knife.image.enabled = false;
            }
            step.completed = true;
            stepComplete++;
            break;
        }

        if (stepComplete == Cut.cutSteps.Count)
        {
            Cut.completed = true;
            _prep?.UpdatePrep();
            SetCanvas(false);
        }
    }
}