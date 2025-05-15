using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName ="Prototype/Item/Data")]
public class ItemSO : ScriptableObject 
{
    [SerializeField] private string _name;
    [SerializeField] private string _id;
    [SerializeField] private string _description;  
    [SerializeField] private AssetReferenceSprite _icon;
    [SerializeField] private AssetReference _gameObject;
    [SerializeField] private ItemInventory.ItemType _itemType;

    public string Name { get => _name;}
    public string Id { get => _id; }
    public Sprite Icon { get => Addressables.LoadAssetAsync<Sprite>(_icon).WaitForCompletion();}
    public GameObject GameObject { get => Addressables.InstantiateAsync(_gameObject).WaitForCompletion();}
    public string Description { get => _description;}
    public ItemInventory.ItemType ItemType { get => _itemType; }
}