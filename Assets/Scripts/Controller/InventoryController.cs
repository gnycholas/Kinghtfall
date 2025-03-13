using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private Dictionary<int, Item> _dictionaryItem = new();
    private WeaponView _SelectedWeapon;
    private ItemView _SelectedItem;
    [SerializeField] private Transform _handLeft;
    [SerializeField] private Transform _handRight;
    [SerializeField] private GameObject _daggerPrefab;
    [SerializeField] private GameObject _potionPrefab;
    private void Awake()
    {
        for(int index = 0; index < 10; index++)
        {
            _dictionaryItem[index] = default;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GetComponent<PlayerController>().EquipItem(new Item() { Amount = 1, Name = "Dagger", Type = ItemType.WEAPON });
        }
    }

    private void OnEnable()
    {
        GetComponent<PlayerController>().OnEquipWeapon.AddListener(EquipeWeapon);
        GetComponent<PlayerController>().OnEquipConsumible.AddListener(EquipConsumible);
        GetComponent<PlayerController>().OnCollectItem.AddListener(AddItem);
        GetComponent<PlayerController>().OnRemoveItem.AddListener(RemoveItem);
    }


    private void OnDisable()
    {
        GetComponent<PlayerController>().OnEquipWeapon.RemoveListener(EquipeWeapon);
        GetComponent<PlayerController>().OnEquipConsumible.RemoveListener(EquipConsumible);
        GetComponent<PlayerController>().OnCollectItem.RemoveListener(AddItem); 
        GetComponent<PlayerController>().OnRemoveItem.RemoveListener(RemoveItem);
    }

    private void EquipConsumible(Item consumible)
    {
        _SelectedItem = Instantiate(_potionPrefab,_handRight).GetComponent<ItemView>(); 
    }

    private void EquipeWeapon(Item weapon)
    { 
        if(weapon.Name == "Dagger")
        {
            _SelectedWeapon = Instantiate(_daggerPrefab, _handRight).GetComponent<WeaponView>();
        }
        _SelectedWeapon.SetOwner(gameObject);
    }
    public void AddItem(Item item)
    {
        foreach (var pair in _dictionaryItem)
        {
            if(pair.Value.Name == item.Name)
            {
                _dictionaryItem[pair.Key] = pair.Value with { Amount = item.Amount + pair.Value.Amount };
                return;
            }
        }
        foreach (var pair in _dictionaryItem)
        {
            if(pair.Value == default)
            {
                _dictionaryItem[pair.Key] = item;
            }
        }
    }
    public void RemoveItem(Item item) 
    {
        foreach (var pair in _dictionaryItem)
        {
            if (pair.Value.Name == item.Name)
            {
                var newItem = pair.Value with { Amount = pair.Value.Amount - item.Amount };
                if(newItem.Amount == 0)
                {
                    _dictionaryItem[pair.Key] = default;
                }
                else
                {
                    _dictionaryItem[pair.Key] = newItem;
                }
                return;
            }
        }
    }
}
