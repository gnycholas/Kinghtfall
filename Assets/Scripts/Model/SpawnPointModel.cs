using UnityEngine;

[CreateAssetMenu(menuName = "Spawn Point Data")]
public class SpawnPointModel : ScriptableObject
{
    [Tooltip("Nome exato da cena onde este spawn existe")]
    public string sceneName;
    [Tooltip("Identificador �nico dentro da cena")]
    public string spawnID;
}
