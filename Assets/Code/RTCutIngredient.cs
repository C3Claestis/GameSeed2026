using System;
using System.Collections.Generic;

[Serializable]
public class RTCutIngredient : RecipeTask
{
    public List<CutStep> cutSteps = new();
}