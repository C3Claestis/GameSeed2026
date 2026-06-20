using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Tooltip("Assigned from Menu Selector")]
    [ReadOnly] public MenuData menuData;

    private TMP_Text _text;

    public Button Button { get; private set; }

    public Action<Menu> OnSelected;

    private void Awake()
    {
        _text = GetComponentInChildren<TMP_Text>();
        Button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (Button) Button.onClick.AddListener(HandleClick);
    }

    private void OnDisable()
    {
        if (Button) Button.onClick.RemoveAllListeners();
    }

    public void SetText(string text)
    {
        if (!_text || text == string.Empty) return;
        _text.text = text;
    }

    public void SetText()
    {
        if (!menuData || !_text) return;
        SetText(menuData.menuName);
    }
    
    private void HandleClick()
    {
        OnSelected?.Invoke(this);
    }
}
