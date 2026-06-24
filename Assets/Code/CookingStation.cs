using UnityEngine;

public class CookingStation : MonoBehaviour, IStation
{
    [field: SerializeField] public int StationId { get; private set; }
}