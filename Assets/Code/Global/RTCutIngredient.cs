using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RTCutIngredient : RecipeTask
{
    public Ingredient ingredient;
    public List<CutStep> cutSteps = new();
    public Sprite resultSprite;

    //Tambahan Jafar
    public enum KnifeType
    {
        Normal,
        Garlic
    }

    public KnifeType knifeType;
}