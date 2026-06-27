using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class StationManager : MonoBehaviour
{
    public static StationManager Instance;

    [SerializeField] private Image _fader;

    private Canvas _faderCanvas;
    private IStation[] _stations;
    private CanvasGroup[] _stationCanvases;
    private CanvasGroup _mainCanvas;

    public int ActiveStation { get; private set; } = 0;
    public CashierStation CashierStation { get; private set; }
    public PrepStation PrepStation { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        TryGetComponent(out _mainCanvas);
        if (_fader)
        {
            if (_fader.TryGetComponent(out _faderCanvas)) _faderCanvas.enabled = false;
        }

        _stations = GetComponentsInChildren<IStation>();
        _stationCanvases = new CanvasGroup[_stations.Length];
        
        for (var i = 0; i < _stations.Length; i++)
        {
            var station = _stations[i];
            var canvas = (station as MonoBehaviour)?.GetComponent<CanvasGroup>();
            if (canvas) _stationCanvases[i] = canvas;
            switch (station)
            {
                case CashierStation cashier:
                    CashierStation = cashier;
                    break;
                case PrepStation prep:
                    PrepStation = prep;
                    break;
            }
        }
    }

    private void Start()
    {
        if (_stations.Length > 0)
        {
            SetCanvasStation(0, true);
            for (var i = 1; i < _stationCanvases.Length; i++) SetCanvasStation(i, false);
        }
    }

    private void SetCanvasStation(int index, bool value)
    {
        if (!_stationCanvases[index]) return;
        _stationCanvases[index].alpha = value ? 1 : 0;
        _stationCanvases[index].interactable = value;
        _stationCanvases[index].blocksRaycasts = value;
        
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
        if (ActiveStation == from && ActiveStation == to) return;
        if (_mainCanvas) _mainCanvas.interactable = false;
        if (from > to)
            FadeStation(false, callback: () =>
            {
                ActiveStation = to;
                SetCanvasStation(from, false);
                SetCanvasStation(to, true);
                FadeStation(true, callback: () =>
                {
                    if (_faderCanvas) _faderCanvas.enabled = false;
                    if (_mainCanvas) _mainCanvas.interactable = true;
                });
            });

        FadeStation(true, callback: () =>
        {
            ActiveStation = to;
            SetCanvasStation(from, false);
            SetCanvasStation(to, true);
            FadeStation(false, true, () =>
            {
                if (_faderCanvas) _faderCanvas.enabled = false;
                if (_mainCanvas) _mainCanvas.interactable = true;
            });
        });
    }

    public void GoToCashierStation() => GoToStation(ActiveStation, CashierStation.StationId);
    public void GoToPrepStation() => GoToStation(ActiveStation, PrepStation.StationId);
}
