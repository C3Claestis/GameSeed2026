using System;
using UnityEngine;

public class Knife : MonoBehaviour
{
    private PrepStation _prep;
    
    private void Start()
    {
        _prep = StationManager.Instance?.GetStation<PrepStation>();
    }

    public void UpdateStep()
    {
        if (!_prep) return;
        _prep.ChopSystem?.UpdateSteps();
    }
}
