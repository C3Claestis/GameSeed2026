using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICutLineDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas canvas;

    [SerializeField] private GraphicRaycaster raycaster;

    [Header("Cut Settings")] [Tooltip("Minimum swipe length in canvas units before a cut fires.")]
    [SerializeField] private float minCutLength = 30f;

    [Tooltip("Number of sample points along the swipe used for raycasting. " +
             "More = catches narrower objects, but slightly more expensive.")]
    [SerializeField]
    [Range(3, 15)]
    private int cutSamples = 7;

    [Header("Debug")] [Tooltip("Draws the last cut line in the Scene view for 2 seconds.")]
    [SerializeField] private bool debugMode;

    private RectTransform _canvasRect;
    private Vector2 _cutStartScreen;
    private Vector2 _cutStartCanvas;
    private bool _isCutting;

    private Vector3 _dbgWorldStart;
    private Vector3 _dbgWorldEnd;
    private float _dbgTimer;

    private void Awake()
    {
        if (canvas == null)
        {
            Debug.LogError("[UICutLineDetector] Canvas reference is not set.", this);
            enabled = false;
            return;
        }

        _canvasRect = canvas.GetComponent<RectTransform>();

        if (raycaster == null)
            raycaster = canvas.GetComponent<GraphicRaycaster>();

        if (raycaster == null)
        {
            Debug.LogError("[UICutLineDetector] No GraphicRaycaster found on Canvas.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        HandleInput(
            Input.GetMouseButtonDown(0),
            Input.GetMouseButtonUp(0),
            Input.mousePosition
        );

        if (debugMode && _dbgTimer > 0f)
            _dbgTimer -= Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        if (!debugMode || _dbgTimer <= 0f) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_dbgWorldStart, _dbgWorldEnd);
        Gizmos.DrawSphere(_dbgWorldStart, 2f);
        Gizmos.DrawSphere(_dbgWorldEnd, 2f);
    }

    private void HandleInput(bool down, bool up, Vector2 screenPos)
    {
        if (down)
        {
            _cutStartScreen = screenPos;
            _cutStartCanvas = ScreenToCanvasLocal(screenPos);
            _isCutting = true;
        }

        if (_isCutting && up)
        {
            _isCutting = false;
            var endCanvas = ScreenToCanvasLocal(screenPos);

            if (Vector2.Distance(_cutStartCanvas, endCanvas) >= minCutLength)
                TryPerformCut(_cutStartScreen, _cutStartCanvas, screenPos, endCanvas);
        }
    }

    private void TryPerformCut(
        Vector2 startScreen, Vector2 startCanvas,
        Vector2 endScreen, Vector2 endCanvas)
    {
        if (debugMode)
        {
            _dbgWorldStart = _canvasRect.TransformPoint(new Vector3(startCanvas.x, startCanvas.y, 0f));
            _dbgWorldEnd = _canvasRect.TransformPoint(new Vector3(endCanvas.x, endCanvas.y, 0f));
            _dbgTimer = 2f;
        }

        var hitObjects = new HashSet<CuttableUIObject>();
        var results = new List<RaycastResult>();
        var ped = new PointerEventData(EventSystem.current);

        for (var i = 0; i < cutSamples; i++)
        {
            var t = (float)i / (cutSamples - 1);
            ped.position = Vector2.Lerp(startScreen, endScreen, t);
            results.Clear();
            raycaster.Raycast(ped, results);

            foreach (var hit in results)
            {
                var cuttable = hit.gameObject.GetComponent<CuttableUIObject>();
                if (cuttable != null && cuttable.CanBeCut())
                    hitObjects.Add(cuttable);
            }
        }

        foreach (var cuttable in hitObjects)
        {
            if (cuttable == null) continue;
            UICutSystem.Cut(cuttable, startCanvas, endCanvas, _canvasRect);
        }
    }

    private Vector2 ScreenToCanvasLocal(Vector2 screenPos)
    {
        var cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect, screenPos, cam, out var localPoint);

        return localPoint;
    }
}