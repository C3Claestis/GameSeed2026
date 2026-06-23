using System;
using System.Collections.Generic;

[Serializable]
public abstract class RecipeTask
{
    public bool completed;
}

[Serializable]
public class RTCutIngredient : RecipeTask
{
    public List<CutStep> cutSteps = new();
}