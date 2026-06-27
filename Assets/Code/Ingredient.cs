using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Ingredient")]
public class Ingredient : ScriptableObject
{
    public string recipeName;

    #if UNITY_EDITOR
    [ContextMenu("Fill Name"), Button("Fill Name")]
    private void FillName()
    {
        recipeName = name;
    }
    #endif
}