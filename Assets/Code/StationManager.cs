using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class StationManager : MonoBehaviour
{
    public static StationManager Instance;

    [SerializeField] private CashierStation _cashierStation;
    [SerializeField] private PrepStation _prepStation;
    [SerializeField] private Image _fader;

    private Canvas _faderCanvas;
    
    public CashierStation CashierStation => _cashierStation;
    public PrepStation PrepStation => _prepStation;
    
    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        if (_fader)
        {
            if (_fader.TryGetComponent(out _faderCanvas)) _faderCanvas.enabled = false;
        }
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void FadeRight()
    {
        FadeStation(true);
    }
    
    public void FadeStation(bool right, bool invert = false, Action callback = null)
    {
        if (!_fader)
        {
            print($"No Fader");
            return;
        }
        if (_faderCanvas) _faderCanvas.enabled = true;
        var targetDirection = right ? Vector3.right : Vector3.left;
        var targetPosition = targetDirection * _fader.rectTransform.rect.width;
        _fader.transform.localPosition = !invert ? targetPosition : Vector3.zero;
        _fader.transform.DOLocalMoveX(!invert ? 0 : targetPosition.x, 1f)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() => callback?.Invoke());
    }

    public void GoToStation(int from, int to)
    {
        if (from > to)
        {
            FadeStation(false, callback: () =>
            {
                FadeStation(true, callback: () =>
                {
                    if (_faderCanvas) _faderCanvas.enabled = false;
                });
            });
        }

        FadeStation(true, callback: () =>
        {
            FadeStation(false, true, callback: () =>
            {
                if (_faderCanvas) _faderCanvas.enabled = false;
            });
        });
    }
}
