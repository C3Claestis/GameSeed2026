using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class CuttableUIObject : MonoBehaviour
{
    [Tooltip("Maximum number of times this piece (or any of its children) can be cut.")]
    [SerializeField] private int maxCutDepth = 3;

    private RectTransform _rectTransform;
    private Sprite _sprite;
    private Color _color = Color.white;
    private List<Vector2> _polygon;
    private Rect _originalBounds;
    private int _cutDepth;
    private bool _initialized;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(Sprite sprite, Color color, List<Vector2> polygon, int cutDepth, Rect originalBounds)
    {
        _rectTransform = GetComponent<RectTransform>();
        _sprite = sprite;
        _color = color;
        _cutDepth = cutDepth;
        _originalBounds = originalBounds;
        _polygon = polygon ?? BuildRectPolygon();
        _initialized = true;
    }

    public bool CanBeCut()
    {
        return EnsureInit() && _cutDepth < maxCutDepth;
    }

    public int CutDepth => _cutDepth;
    public Rect OriginalBounds => _originalBounds;
    public RectTransform RectTf => _rectTransform ??= GetComponent<RectTransform>();

    public Sprite GetSprite()
    {
        EnsureInit();
        return _sprite;
    }

    public Color GetColor()
    {
        EnsureInit();
        return _color;
    }

    public List<Vector2> GetPolygon()
    {
        EnsureInit();
        return _polygon ??= BuildRectPolygon();
    }

    private bool EnsureInit()
    {
        if (_initialized) return true;

        var img = GetComponent<Image>();
        if (img == null)
        {
            Debug.LogWarning($"[CuttableUIObject] {name} has no Image component.", this);
            return false;
        }

        if (img.sprite == null)
        {
            Debug.LogWarning($"[CuttableUIObject] {name} Image has no sprite assigned.", this);
            return false;
        }

        _sprite = img.sprite;

        _color = img.color;
        if (img.material != null && img.material != img.defaultMaterial && img.material.HasProperty("_Color"))
            _color *= img.material.GetColor("_Color");

        var r = _rectTransform.rect;
        if (img.preserveAspect)
            r = FitSpriteAspect(r, _sprite);

        _originalBounds = r;
        _polygon = BuildRectPolygon(r);
        _initialized = true;
        return true;
    }

    private static Rect FitSpriteAspect(Rect rect, Sprite sprite)
    {
        if (rect.width < 0.001f || rect.height < 0.001f) return rect;

        var spriteAspect = sprite.rect.width / sprite.rect.height;
        var rectAspect = rect.width / rect.height;

        if (spriteAspect > rectAspect)
        {
            var h = rect.width / spriteAspect;
            return new Rect(rect.xMin, rect.center.y - h * 0.5f, rect.width, h);
        }

        var w = rect.height * spriteAspect;
        return new Rect(rect.center.x - w * 0.5f, rect.yMin, w, rect.height);
    }

    private List<Vector2> BuildRectPolygon()
    {
        return BuildRectPolygon(_rectTransform.rect);
    }

    private List<Vector2> BuildRectPolygon(Rect r)
    {
        return new List<Vector2>
        {
            new(r.xMin, r.yMin),
            new(r.xMax, r.yMin),
            new(r.xMax, r.yMax),
            new(r.xMin, r.yMax)
        };
    }
}