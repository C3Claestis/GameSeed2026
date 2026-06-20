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
    private Canvas[] _stationCanvases;

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
        
        if (_fader)
        {
            if (_fader.TryGetComponent(out _faderCanvas)) _faderCanvas.enabled = false;
        }

        _stations = GetComponentsInChildren<IStation>();
        _stationCanvases = new Canvas[_stations.Length];
        
        for (var i = 0; i < _stations.Length; i++)
        {
            var station = _stations[i];
            var canvas = (station as MonoBehaviour)?.GetComponent<Canvas>();
            if (canvas)
            {
                _stationCanvases[i] = canvas;
            }
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
            if (_stationCanvases[0]) _stationCanvases[0].enabled = true;
            for (var i = 1; i < _stationCanvases.Length; i++)
            {
                if (_stationCanvases[i]) _stationCanvases[i].enabled = false;
            }
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
            FadeStation(false, callback: () =>
            {
                ActiveStation = to;
                _stationCanvases[from].enabled = false;
                _stationCanvases[to].enabled = true;
                FadeStation(true, callback: () =>
                {
                    if (_faderCanvas) _faderCanvas.enabled = false;
                });
            });

        FadeStation(true, callback: () =>
        {
            ActiveStation = to;
            _stationCanvases[from].enabled = false;
            _stationCanvases[to].enabled = true;
            FadeStation(false, true, () =>
            {
                if (_faderCanvas) _faderCanvas.enabled = false;
            });
        });
    }

    public void GoToCashierStation() => GoToStation(ActiveStation, CashierStation.StationId);
    public void GoToPrepStation() => GoToStation(ActiveStation, PrepStation.StationId);
}
