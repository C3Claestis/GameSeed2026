using UnityEngine;

[CreateAssetMenu(fileName = "New Customer", menuName ="Customer Data")]
public class CustomerData : ScriptableObject
{
    public string customerName;
    public Sprite customerSprite;
    public float patience;
}
