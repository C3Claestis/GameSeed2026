using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    [SerializeField] private float fadeDuration;

    private float _patience;
    private Image _image;
    private TMP_Text _text;
    private Sequence _fadeSequence;
    private Tweener _patienceTween;

    public float Patience => _patience;
    public CustomerData CustomerData { get; private set; }
    public MenuData MenuData { get; set; }
    public float CurrentPatience { get; set; }
    
    public Action<MenuData> OnCustomerOrderChanged;
    public Action OnCustomerOutOfPatience;
    
    private void Awake()
    {
        _image = GetComponentInChildren<Image>();
        if (_image)
        {
            _image.preserveAspect = true;
            _image.color = new Color(255, 255, 255, 0);
        }
        _text = transform.parent?.GetComponentInChildren<TMP_Text>();
    }

    private void Enter()
    {
        const float fadeEnd = 1f;
        
        _fadeSequence?.Kill();
        _patienceTween?.Kill();
        
        _fadeSequence = DOTween.Sequence()
            .SetEase(Ease.InOutQuart)
            .OnComplete(AfterFade);
        
        if (_image)
        {
            _image.color = new Color(255, 255, 255, 0);
            var imageAnim = _image.DOFade(fadeEnd, fadeDuration);
            _fadeSequence.Join(imageAnim);
        }
        if (_text)
        {
            _text.text = string.Empty;
            var cg = _text.GetComponentInParent<CanvasGroup>();
            if (cg)
            {
                cg.alpha = 0;
                var textAnim = cg.DOFade(fadeEnd, fadeDuration);
                _fadeSequence.Join(textAnim);
            }
            else
            {
                _text.color = new Color(255, 255, 255, 0);
                var textAnim = _text.DOFade(fadeEnd, fadeDuration);
                _fadeSequence.Join(textAnim);
            }
        }
    }

    private void AfterFade()
    {
        print($"Fade complete");
        MenuData = MenuManager.Instance?.GetRandomMenu();
        if (!MenuData) return;
        _text.text = $"I want {MenuData.menuName}";
        OnCustomerOrderChanged?.Invoke(MenuData);
        _patience = CustomerData.patience;
        _patienceTween = DOVirtual.Float(0f, _patience, _patience, value => CurrentPatience = value)
            .OnComplete(() => OnCustomerOutOfPatience?.Invoke());
    }

    public void Enter(CustomerData data)
    {
        if (!data) return;
        SetData(data);
        if (_image) _image.sprite = CustomerData.customerSprite; 
        Enter();
    }

    public void SetData(CustomerData data)
    {
        CustomerData = data;
    }
}
