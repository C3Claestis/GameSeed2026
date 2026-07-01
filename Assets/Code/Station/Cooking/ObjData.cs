using System;
using UnityEngine;

public class ObjData : MonoBehaviour
{
    public enum MenuType
    {
        NasiGoreng,
        MieGoreng,
        Steak
    }

    [Serializable]
    public class SpawnData
    {
        public Transform point;
        public GameObject prefab;
    }

    [Serializable]
    public class MenuSpawn
    {
        public MenuType menu;
        public SpawnData[] objects;
    }

    [SerializeField] private MenuSpawn[] menuSpawns;

    public MenuSpawn GetMenu(MenuType menu)
    {
        foreach (var item in menuSpawns)
        {
            if (item.menu == menu)
                return item;
        }

        return null;
    }
}