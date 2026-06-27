using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RTCutIngredient : RecipeTask
{
    public List<CutStep> cutSteps = new();
    public Sprite resultSprite;
}