using UnityEngine;

public class ChopSystem : MonoBehaviour
{
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private GameObject knifePrefab;
    [SerializeField] private CanvasGroup prepCanvas;
    [SerializeField] private CanvasGroup choppingCanvas;
    [SerializeField] private Transform cuttingBoard;

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
    
    
}
