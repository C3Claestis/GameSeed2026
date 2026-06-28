using System.Collections.Generic;
using UnityEngine;

public static class MeshUtils
{
    public static (List<Vector2> left, List<Vector2> right) SlicePolygon(
        List<Vector2> polygon, Vector2 linePoint, Vector2 lineDir)
    {
        var left = new List<Vector2>();
        var right = new List<Vector2>();

        for (var i = 0; i < polygon.Count; i++)
        {
            var curr = polygon[i];
            var next = polygon[(i + 1) % polygon.Count];

            var currLeft = IsLeft(curr, linePoint, lineDir);
            var nextLeft = IsLeft(next, linePoint, lineDir);

            if (currLeft) left.Add(curr);
            else right.Add(curr);

            if (currLeft != nextLeft)
                if (TryGetIntersection(curr, next, linePoint, linePoint + lineDir, out var hit))
                {
                    left.Add(hit);
                    right.Add(hit);
                }
        }

        return (left, right);
    }

    public static bool IsLeft(Vector2 point, Vector2 linePoint, Vector2 lineDir)
    {
        return lineDir.x * (point.y - linePoint.y) - lineDir.y * (point.x - linePoint.x) > 0f;
    }

    public static bool TryGetIntersection(
        Vector2 a1, Vector2 a2,
        Vector2 b1, Vector2 b2,
        out Vector2 intersection)
    {
        var d1 = a2 - a1;
        var d2 = b2 - b1;
        var cross = d1.x * d2.y - d1.y * d2.x;

        intersection = Vector2.zero;
        if (Mathf.Abs(cross) < 1e-8f) return false;

        var delta = b1 - a1;
        var t = (delta.x * d2.y - delta.y * d2.x) / cross;
        intersection = a1 + t * d1;
        return true;
    }
}