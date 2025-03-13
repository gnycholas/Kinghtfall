using UnityEngine;

[CreateAssetMenu(menuName ="Prototype/Inventory/Item")]
public class ItemSO : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _icon;
    [SerializeField] private GameObject _item;

    public string Name { get => _name;}
    public Sprite Icon { get => _icon;}
    public GameObject Item { get => _item;}
}
