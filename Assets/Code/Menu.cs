using TMPro;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [Tooltip("Assigned from Menu Selector")]
    public MenuData menuData;

    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponentInChildren<TMP_Text>();
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
}
