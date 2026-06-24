using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StationManager : MonoBehaviour
{
    public static StationManager Instance;

    [SerializeField] private Image _fader;

    private Canvas _faderCanvas;
    private CanvasGroup[] _stationCanvases;
    private CanvasGroup _mainCanvas;
    private readonly Dictionary<Type, IStation> _stationDictionary = new();
    private IStation[] _stationIndexes;
    
    public int ActiveStation { get; private set; } = 0;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        TryGetComponent(out _mainCanvas);
        if (_fader && _fader.TryGetComponent(out _faderCanvas))
        {
            _faderCanvas.enabled = false;
        }

        var stations = GetComponentsInChildren<IStation>();
        _stationCanvases = new CanvasGroup[stations.Length];
        _stationIndexes = new IStation[stations.Length];
        
        for (var i = 0; i < stations.Length; i++)
        {
            var station = stations[i];
            if (station == null) continue;
            _stationIndexes[station.StationId] = station;

            var canvas = (station as MonoBehaviour)?.GetComponent<CanvasGroup>();
            if (canvas) _stationCanvases[i] = canvas;

            var stationType = station.GetType();
            if (!_stationDictionary.TryAdd(stationType, station))
            {
                Debug.LogWarning($"Multiple stations of type {stationType.Name} found!");
            }
        }
    }

    private void Start()
    {
        if (_stationIndexes.Length > 0)
        {
            SetCanvasStation(0, true);
            for (var i = 1; i < _stationCanvases.Length; i++) SetCanvasStation(i, false);
        }
    }

    public T GetStation<T>() where T : class, IStation
    {
        if (_stationDictionary.TryGetValue(typeof(T), out var station))
        {
            return station as T;
        }
        Debug.LogError($"Station of type {typeof(T).Name} not found!");
        return null;
    }

    private void SetCanvasStation(int index, bool value)
    {
        if (index < 0 || index >= _stationCanvases.Length || !_stationCanvases[index]) return;
        _stationCanvases[index].alpha = value ? 1 : 0;
        _stationCanvases[index].interactable = value;
        _stationCanvases[index].blocksRaycasts = value;
    }

    public void FadeStation(bool right, bool invert = false, Action callback = null)
    {
        if (!_fader)
        {
            print("No Fader");
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
        var right = to > from;
        if (from > to)
        {
            FadeStation(right, callback: () =>
            {
                ActiveStation = to;
                SetCanvasStation(from, false);
                SetCanvasStation(to, true);
                _stationIndexes[from].OnClose();
                _stationIndexes[to].OnOpen();
                FadeStation(!right, true, callback: () =>
                {
                    if (_faderCanvas) _faderCanvas.enabled = false;
                    if (_mainCanvas) _mainCanvas.interactable = true;
                });
            });
            return;
        }
        FadeStation(right, callback: () =>
        {
            ActiveStation = to;
            SetCanvasStation(from, false);
            SetCanvasStation(to, true);
            _stationIndexes[from].OnClose();
            _stationIndexes[to].OnOpen();
            FadeStation(!right, true, () =>
            {
                if (_faderCanvas) _faderCanvas.enabled = false;
                if (_mainCanvas) _mainCanvas.interactable = true;
            });
        });
    }

    public void GoToStation<T>() where T : class, IStation
    {
        var targetStation = GetStation<T>();
        if (targetStation != null)
        {
            GoToStation(ActiveStation, targetStation.StationId);
        }
    }

    public void GoToStation(int stationId)
    {
        GoToStation(ActiveStation, stationId);
    }
}