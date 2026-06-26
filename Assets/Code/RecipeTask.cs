using System;
using System.Collections.Generic;

[Serializable]
public abstract class RecipeTask
{
    public bool completed;
    public string description;
}

[Serializable]
public class RTCutIngredient : RecipeTask
{
    public List<CutStep> cutSteps = new();
}