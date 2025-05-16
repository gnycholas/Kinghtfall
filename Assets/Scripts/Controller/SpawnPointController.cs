using UnityEngine;

public class SpawnPointController : MonoBehaviour
{
    [Tooltip("Dados deste ponto de spawn")]
    [SerializeField] private SpawnPointModel _data; 
    [SerializeField] private Transform _targetPosition;

    public Transform Target => _targetPosition;
    public int Id => _data.HashId;
}
