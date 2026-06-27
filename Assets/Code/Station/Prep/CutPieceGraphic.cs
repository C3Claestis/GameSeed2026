using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class CutPieceGraphic : MaskableGraphic
{
    private List<Vector2> _polygon;
    private Sprite _sprite;
    private Rect _originalBounds;

    private Color32 _tint = Color.white;

    private bool _ready;

    public override Texture mainTexture
        => _sprite != null ? _sprite.texture : base.mainTexture;

    public void Initialize(List<Vector2> polygon, Sprite sprite, Rect originalBounds, Color tint)
    {
        _polygon = polygon;
        _sprite = sprite;
        _originalBounds = originalBounds;
        _tint = tint;
        _ready = true;

        SetMaterialDirty();
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (!_ready || _polygon == null || _polygon.Count < 3 || _sprite == null)
            return;

        if (_originalBounds.width < 0.001f || _originalBounds.height < 0.001f)
        {
            Debug.LogWarning($"[CutPieceGraphic] originalBounds on {name} has near-zero size. " +
                             "Check that the source Image's RectTransform has a non-zero size at cut time.");
            return;
        }

        var uv = DataUtility.GetOuterUV(_sprite);

        for (var i = 0; i < _polygon.Count; i++)
        {
            var vert = UIVertex.simpleVert;
            vert.position = _polygon[i];
            vert.color = _tint;

            var nx = Mathf.InverseLerp(_originalBounds.xMin, _originalBounds.xMax, _polygon[i].x);
            var ny = Mathf.InverseLerp(_originalBounds.yMin, _originalBounds.yMax, _polygon[i].y);

            vert.uv0 = new Vector2(
                Mathf.Lerp(uv.x, uv.z, nx),
                Mathf.Lerp(uv.y, uv.w, ny)
            );

            vh.AddVert(vert);
        }

        for (var i = 1; i < _polygon.Count - 1; i++)
            vh.AddTriangle(0, i, i + 1);
    }
}