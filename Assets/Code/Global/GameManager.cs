using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float globalTimer = 200f;
    [SerializeField] private Text timerText;

    private void Start()
    {
        UpdateTimerUI();
        InvokeRepeating(nameof(ReduceTimer), 1f, 1f);
    }

    private void ReduceTimer()
    {
        globalTimer--;

        UpdateTimerUI();

        if (globalTimer > 0)
        {
            
        }

        if (globalTimer <= 0)
        {
            globalTimer = 0;
            UpdateTimerUI();

            CancelInvoke(nameof(ReduceTimer));

            // TODO: End Game
        }
    }

    private void UpdateTimerUI()
    {
        if (!timerText)
            return;

        int minutes = Mathf.FloorToInt(globalTimer / 60f);
        int seconds = Mathf.FloorToInt(globalTimer % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}