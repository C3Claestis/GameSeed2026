using UnityEngine;
using UnityEngine.Events;

public class IndicatorCooking : MonoBehaviour
{
    [Header("Needle")]
    [SerializeField] private Transform needle;
    [SerializeField] private float rotateSpeed = 180f; // derajat per detik

    [Header("Indicator")]
    [SerializeField] private int indicatorCount = 5;

    public UnityEvent<int> OnIndicatorChanged;

    private float currentAngle;
    private int currentIndicator = -1;

    public int CurrentIndicator => currentIndicator;
    public float CurrentAngle => currentAngle;

    private void Update()
    {
        RotateNeedle();
        UpdateIndicator();
    }

    private void RotateNeedle()
    {
        // Rotate clockwise
        currentAngle += rotateSpeed * Time.deltaTime;

        // Kembali ke 0 setelah 360
        currentAngle = Mathf.Repeat(currentAngle, 360f);

        // Unity positif = CCW, maka negatif agar CW
        needle.localRotation = Quaternion.Euler(0f, 0f, -currentAngle);
    }

    private void UpdateIndicator()
    {
        float section = 360f / indicatorCount;

        int indicator = Mathf.FloorToInt(currentAngle / section);

        if (indicator != currentIndicator)
        {
            currentIndicator = indicator;
            OnIndicatorChanged?.Invoke(currentIndicator);

            Debug.Log($"Indicator : {currentIndicator}");
        }
    }
}