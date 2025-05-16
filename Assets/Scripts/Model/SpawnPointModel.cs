using UnityEngine;

[CreateAssetMenu(menuName = "Spawn Point Data")]
public class SpawnPointModel : ScriptableObject
{
    [Tooltip("Nome exato da cena onde este spawn existe")]
    [SerializeField] private string sceneName;
    [Tooltip("Identificador único dentro da cena")]
    [SerializeField] private string spawnID;
    private int _hash;
    public int HashId { get => _hash;}
    public string SceneName { get => sceneName;}

    private void OnValidate()
    {
        _hash = spawnID.GetHashCode();
    }
}
