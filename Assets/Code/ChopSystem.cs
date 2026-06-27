using UnityEngine;
using UnityEngine.UI;

public class ChopSystem : MonoBehaviour
{
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private GameObject knifePrefab;
    [SerializeField] private CanvasGroup prepCanvas;
    [SerializeField] private CanvasGroup choppingCanvas;
    [SerializeField] private Transform cuttingBoard;

    private Image _target;
    private Button _knife;
    
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
        
        var targetGo = Instantiate(ingredientPrefab, cuttingBoard);
        _target = targetGo.GetComponent<Image>();
        _knife = Instantiate(knifePrefab, targetGo.transform).GetComponent<Button>();
        _target.sprite = menu.cutSteps[0]?.sprite;
        var knifePos = new Vector3(menu.cutSteps[0]?.knifePosition.x ?? 0, 0, menu.cutSteps[0]?.knifePosition.y ?? 0);
        _knife.transform.localPosition = knifePos;
    }
}
