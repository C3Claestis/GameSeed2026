using UnityEngine;

public class ChopSystem : MonoBehaviour
{
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private GameObject knifePrefab;
    [SerializeField] private CanvasGroup choppingCanvas;

    public void SetCanvas(bool value)
    {
        choppingCanvas.interactable = value;
        choppingCanvas.alpha = value ? 1f : 0;
    }
}
