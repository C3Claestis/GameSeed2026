using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CTGetIngredient : CookTask
{
    public Ingredient ingredient;
    public Sprite indicator;

    //Tambahan Jafar
    public enum IndicatorCook
    {
        Green,
        Yellow,
        Orange,
        Pink,
        Red
    }

    public IndicatorCook indicatorCook;

    public int RequiredIndicator => (int)indicatorCook;
}