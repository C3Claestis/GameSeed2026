using System.Collections.Generic;
using UnityEngine;

public static class UICutSystem
{
    public static float MinPieceArea = 400f;
    public static float PieceSeparation = 1f;

    public static event System.Action<GameObject, GameObject> OnCut;

    public static (GameObject pieceA, GameObject pieceB) Cut(
        CuttableUIObject target,
        Vector2 canvasLocalStart,
        Vector2 canvasLocalEnd,
        RectTransform canvasRect)
    {
        if (target == null || !target.CanBeCut()) return (null, null);

        var sp = target.GetSprite();
        var tint = target.GetColor();

        if (sp == null)
        {
            Debug.LogWarning($"[UICutSystem] {target.name} has no sprite.", target);
            return (null, null);
        }

        var originalBounds = target.OriginalBounds;
        if (originalBounds.width < 0.001f || originalBounds.height < 0.001f)
        {
            Debug.LogWarning($"[UICutSystem] {target.name} bounds near-zero (layout not settled). Cut skipped.",
                target);
            return (null, null);
        }

        var rt = target.RectTf;

        var worldStart = canvasRect.TransformPoint(new Vector3(canvasLocalStart.x, canvasLocalStart.y, 0f));
        var worldEnd = canvasRect.TransformPoint(new Vector3(canvasLocalEnd.x, canvasLocalEnd.y, 0f));

        Vector2 localStart = rt.InverseTransformPoint(worldStart);
        Vector2 localEnd = rt.InverseTransformPoint(worldEnd);
        var lineDir = (localEnd - localStart).normalized;

        if (lineDir == Vector2.zero)
        {
            Debug.LogWarning("[UICutSystem] Cut line has zero length after coordinate conversion.");
            return (null, null);
        }

        const float Extend = 10000f;
        var extStart = localStart - lineDir * Extend;

        var polygon = target.GetPolygon();
        var (leftPoly, rightPoly) = MeshUtils.SlicePolygon(polygon, extStart, lineDir);

        if (leftPoly.Count < 3 || rightPoly.Count < 3)
        {
            Debug.Log($"[UICutSystem] Cut on {target.name} missed the polygon — no pieces spawned.");
            return (null, null);
        }

        if (!HasSufficientArea(leftPoly) || !HasSufficientArea(rightPoly))
        {
            Debug.Log(
                $"[UICutSystem] Cut on {target.name} produced a piece below MinPieceArea ({MinPieceArea} units²). Ignored.");
            return (null, null);
        }

        var nextDepth = target.CutDepth + 1;

        var cutNormal = new Vector2(-lineDir.y, lineDir.x);

        var pieceA = SpawnPiece("Piece_A", leftPoly, sp, tint, rt, nextDepth, originalBounds,
            cutNormal * PieceSeparation);
        var pieceB = SpawnPiece("Piece_B", rightPoly, sp, tint, rt, nextDepth, originalBounds,
            -cutNormal * PieceSeparation);

        Object.Destroy(target.gameObject);

        OnCut?.Invoke(pieceA, pieceB);
        return (pieceA, pieceB);
    }

    private static GameObject SpawnPiece(
        string name,
        List<Vector2> polygon,
        Sprite sprite,
        Color tint,
        RectTransform original,
        int cutDepth,
        Rect originalBounds,
        Vector2 separation)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(original.parent, false);
        go.transform.SetSiblingIndex(original.GetSiblingIndex());

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = original.anchorMin;
        rt.anchorMax = original.anchorMax;
        rt.pivot = original.pivot;
        rt.sizeDelta = original.sizeDelta;
        rt.anchoredPosition = original.anchoredPosition + separation;
        rt.localRotation = original.localRotation;
        rt.localScale = original.localScale;

        var graphic = go.AddComponent<CutPieceGraphic>();
        graphic.raycastTarget = true;
        graphic.Initialize(polygon, sprite, originalBounds, tint);

        var cuttable = go.AddComponent<CuttableUIObject>();
        cuttable.Initialize(sprite, tint, polygon, cutDepth, originalBounds);

        return go;
    }

    public static float MinPieceThickness = 4f;

    private static bool HasSufficientArea(List<Vector2> polygon)
    {
        var area = 0f;
        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;

        for (var i = 0; i < polygon.Count; i++)
        {
            var a = polygon[i];
            var b = polygon[(i + 1) % polygon.Count];
            area += a.x * b.y - b.x * a.y;

            if (a.x < minX) minX = a.x;
            if (a.x > maxX) maxX = a.x;
            if (a.y < minY) minY = a.y;
            if (a.y > maxY) maxY = a.y;
        }

        if (Mathf.Min(maxX - minX, maxY - minY) < MinPieceThickness)
            return false;

        return Mathf.Abs(area) * 0.5f >= MinPieceArea;
    }
}