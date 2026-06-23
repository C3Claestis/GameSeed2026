using Demo;
using UnityEngine;

[CreateAssetMenu(fileName = "New Menu", menuName = "Menu Data")]
public class MenuData : ScriptableObject
{
    public string menuName;
    public Sprite menuSprite;
    [SerializeReference, SRDemo(typeof(RecipeTask))] public RecipeTask[] recipesTask;
}