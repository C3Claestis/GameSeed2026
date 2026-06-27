using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float globalTimer = 200f;

    [SerializeField] private GameObject pembeli;

    private CustomerState currentState = CustomerState.None;

    private void Start()
    {
        InvokeRepeating(nameof(ReduceTimer), 1f, 1f);
    }

    private void ReduceTimer()
    {
        globalTimer--;

        if (globalTimer > 0 && currentState == CustomerState.None)
        {
            StartCoroutine(SpawnPembeli());
        }

        if (globalTimer <= 0)
        {
            globalTimer = 0;
            CancelInvoke(nameof(ReduceTimer));
        }
    }

    private IEnumerator SpawnPembeli()
    {
        currentState = CustomerState.Waiting;

        // Jeda sebelum muncul
        yield return new WaitForSeconds(2f);

        currentState = CustomerState.Appearing;

        CanvasGroup canvas = pembeli.GetComponent<CanvasGroup>();

        if (canvas != null)
        {
            yield return StartCoroutine(FadeCanvas(canvas, 0, 1, 0.5f));
        }

        currentState = CustomerState.Active;
    }

    private IEnumerator FadeCanvas(CanvasGroup canvas, float start, float end, float duration)
    {
        float elapsed = 0f;

        canvas.alpha = start;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        canvas.alpha = end;
    }

    public void ServeCustomer()
    {
        if (currentState != CustomerState.Active)
            return;

        currentState = CustomerState.Served;

        CanvasGroup canvas = pembeli.GetComponent<CanvasGroup>();

        if (canvas != null)
        {
            canvas.alpha = 0;
        }

        // Reset agar bisa spawn lagi
        currentState = CustomerState.None;
    }
}