using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class InventoryController : MonoBehaviour
{
    public UnityEvent<Weapon> OnWeaponEquip;
    public UnityEvent<Consumible> OnConsumibleEquip; 
    private LinkedList<HUDItemView> _inventory = new();
    private LinkedListNode<HUDItemView> _selectedItem;
    [SerializeField] private RectTransform _container;
    [Inject] private HUDItemView.Factory _hUDItemViewFactory;
    public void Equip(Weapon weapon)
    {
        OnWeaponEquip?.Invoke(weapon);
    }
    public void Equip(Consumible consumible)
    {
        OnConsumibleEquip?.Invoke(consumible);
    }

    public void Select(HUDItemView item)
    {
        item.Equip(this);
    }

    private void NextItem()
    {
        if(_selectedItem.Next != null)
        {
            _selectedItem = _selectedItem.Next;

        }
        else
        {
            _selectedItem = _inventory.First;
        }
        _selectedItem.Value.Select();
    }
    private void PreviusItem()
    {
        if (_selectedItem.Previous != null)
        {
            _selectedItem = _selectedItem.Previous;

        }
        else
        {
            _selectedItem = _inventory.Last;
        }
        _selectedItem.Value.Select();
    }
    public void Collect(ItemCollectibleSO item, int amount)
    {
        var hudItem = _hUDItemViewFactory.Create(item,amount);
        hudItem.transform.SetParent(_container);
    } 
} 
