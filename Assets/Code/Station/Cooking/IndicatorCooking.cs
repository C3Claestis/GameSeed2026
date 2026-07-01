using System;
using UnityEngine;

public class IndicatorCooking : MonoBehaviour
{
    [Serializable]
    public class IndicatorRange
    {
        public int indicator;
        [Range(0, 359)] public float minAngle;
        [Range(0, 359)] public float maxAngle;
    }

    [Header("Needle")]
    [SerializeField] private Transform needle;
    [SerializeField] private float rotateSpeed = 180f;

    [Header("Indicator Ranges")]
    [SerializeField] private IndicatorRange[] ranges;

    private float currentAngle;
    private bool isRunning;

    public int CurrentIndicator { get; private set; } = 0;

    private void Update()
    {
        if (!isRunning) return;

        RotateNeedle();
        UpdateIndicator();
        Debug.Log("Current Indicator : " + CurrentIndicator);
    }

    public void StartIndicator()
    {
        currentAngle = 0f;
        CurrentIndicator = 0;
        needle.localRotation = Quaternion.identity;
        isRunning = true;
    }

    public void StopIndicator()
    {
        isRunning = false;
        currentAngle = 0f;
        CurrentIndicator = -1;
        needle.localRotation = Quaternion.identity;
    }

    private void RotateNeedle()
    {
        currentAngle += rotateSpeed * Time.deltaTime;
        currentAngle %= 360f;

        needle.localRotation = Quaternion.Euler(0, 0, -currentAngle);
    }

    private void UpdateIndicator()
    {
        foreach (var range in ranges)
        {
            bool inRange;

            if (range.minAngle <= range.maxAngle)
            {
                // Range normal, misalnya 90 - 180
                inRange = currentAngle >= range.minAngle &&
                          currentAngle < range.maxAngle;
            }
            else
            {
                // Range melewati 360, misalnya 341 - 72
                inRange = currentAngle >= range.minAngle ||
                          currentAngle < range.maxAngle;
            }

            if (inRange)
            {
                if (CurrentIndicator != range.indicator)
                {
                    CurrentIndicator = range.indicator;
                    Debug.Log($"Indicator : {CurrentIndicator}");
                }
                return;
            }
        }

        CurrentIndicator = -1;
    }
}