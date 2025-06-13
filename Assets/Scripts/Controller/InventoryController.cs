using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class InventoryController : MonoBehaviour
{
    [Inject] private WeaponFactory _weaponFactory;
    [Inject] private ConsumibleFactory _consumibleFactory;  
    public UnityEvent<Weapon> OnWeaponEquip;  
    public UnityEvent OnWeaponUnEquip; 
    public UnityEvent<Consumible> OnConsumibleEquip; 
    public UnityEvent OnConsumibleUnEquip; 
    public UnityEvent<ItemInventory> OnUpdateInventory;
    [Inject] private List<ItemInventory> _items;

    public void UnEquip(int index)
    {
        var item = _items.FirstOrDefault(x=>x.Position == index);
        if(item.Type == ItemInventory.ItemType.EQUIPMENT)
        {
            UnEquipWeapon();
        }
        else if(item.Type == ItemInventory.ItemType.CONSUMIBLE)
        {
            UnEquipConsumible();
        }
    }
    public void Equip(int index)
    {
        var item = _items.FirstOrDefault(x => x.Position == index);
        if (item.Type == ItemInventory.ItemType.EQUIPMENT)
        {
            EquipWeapon(item);
        }else if(item.Type == ItemInventory.ItemType.CONSUMIBLE)
        {
            EquipConsumible(item);
        }
    }
    private void EquipWeapon(ItemInventory weapon)
    { 
        var weaponSelected = _weaponFactory.Create(weapon.Info); 
        OnWeaponEquip?.Invoke(weaponSelected);
    }
    private void UnEquipWeapon()
    {
        OnWeaponUnEquip?.Invoke(); 
    }
    private void EquipConsumible(ItemInventory item)
    { 
        var itemSelected = _consumibleFactory.Create(item.Info);
        OnConsumibleEquip?.Invoke(itemSelected);
    }
    private void UnEquipConsumible()
    {
        OnConsumibleUnEquip?.Invoke();
    }

    public void Select(HUDItemView item)
    {
        item.Equip(this);
    } 
    public void Collect(ItemSO item, int amount)
    {
        var selectedItem = _items.FirstOrDefault(x => x.Equals(item.Id)); 
        if (selectedItem != default)
        {
            var index = _items.IndexOf(selectedItem); 
            selectedItem = selectedItem with { Amount = selectedItem.Amount+amount };
            _items[index] = selectedItem; 
        }
        else
        {
            selectedItem = new ItemInventory(item, amount, _items.Count + 1);
            _items.Add(selectedItem);  
        }
        UpdateInventory(selectedItem); 
    }

    public Tuple<bool,string> CheckItem(ItemSO requiredItem, int amount)
    {
        var item = _items.FirstOrDefault(x=>x.Equals(requiredItem.Id));
        if (item == default)
        {
            return new Tuple<bool, string>(false, $"Você não possui {requiredItem.Name}");
        }
        else
        {
            if(item.Amount - amount < 0)
            {
                return new Tuple<bool, string>(false, $"Você não possui {requiredItem.Name} suficiente");
            }
            else
            {
                return new Tuple<bool, string>(true, "");
            }
        }
    }

    public void ConsumeItem(PlayerController player)
    {
        var selectedItem = _items.FirstOrDefault(x => x.Equals(player.CurrentItem.Info.Id));
        if(selectedItem != default)
        {
            if ((selectedItem.Amount - 1) >= 0)
            {
                var index = _items.IndexOf(selectedItem);
                _items[index] = selectedItem with { Amount = selectedItem.Amount - 1 };
                UpdateInventory(_items[index]);
            } 
        } 
    }

    private void UpdateInventory(ItemInventory item)
    {
        OnUpdateInventory?.Invoke(item);
    }
} 
