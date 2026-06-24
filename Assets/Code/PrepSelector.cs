using TMPro;
using UnityEngine;

public class PrepSelector : MonoBehaviour
{
    [SerializeField] private TMP_Text _menuName;

    public void UpdateMenuName(string text)
    {
        if (!_menuName) return;
        _menuName.text = text;
    }
}
