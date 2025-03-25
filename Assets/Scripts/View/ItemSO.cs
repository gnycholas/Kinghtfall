using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName ="Prototype/Item/Data")]
public class ItemSO : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private AssetReferenceSprite _icon;
    [SerializeField] private AssetReference _gameObject;

    public string Name { get => _name;}
    public Sprite Icon { get => Addressables.LoadAssetAsync<Sprite>(_icon).WaitForCompletion();}
    public GameObject GameObject { get => Addressables.InstantiateAsync(_gameObject).WaitForCompletion();}
}