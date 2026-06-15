using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private List<MenuData> menuData = new();
    
    public IReadOnlyList<MenuData> MenuData => menuData;
    
    public static MenuManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public MenuData GetRandomMenu()
    {
        if (menuData.Count == 0) return null;
        var index = Random.Range(0, menuData.Count);
        return menuData[index];
    }
}