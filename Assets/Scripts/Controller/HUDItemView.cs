using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HUDItemView : MonoBehaviour
{
    private static Action onSelect;
    private ItemSO _item;
    private int _amount;
    private int _position;
    private bool _equiped;
    [Inject] private ItemFactory _itemFactory; 
    [SerializeField] private Image _moldure;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _amountView;

    public string Name =>_item.Name;
    public int Position =>_position;

    public int Amount
    {
        get => _amount; set
        {
            _amount = value;
            if (Amount == 1)
            {
                _amountView.text = "";
                return;
            }
            _amountView.text = $"X{Amount}";
        }
    }

    public bool Equiped { get => _equiped; }

    private void OnEnable()
    {
        onSelect += Deselect;
    }
    private void OnDisable()
    {
        onSelect -= Deselect;
    }
    public void Select()
    {
        onSelect?.Invoke();
        _moldure.enabled = true;
    }
    private void Deselect()
    {
        _moldure.enabled = false;
    }
    public void Equip(InventoryController inventoryController)
    {
        _icon.color = Color.white;
        _equiped = true;
        inventoryController.Equip(_position);
    } 
    public void UnEquip(InventoryController inventoryController)
    {
        _equiped = false;
        _icon.color = new Color(1, 1, 1, 0.25f);
        inventoryController.UnEquip(_position); 
    }
    public void Setup(string item, int amount, int index)
    {
        _item = _itemFactory.Create(item);
        _position = index;
        _amount = amount;
        if(amount > 1)
        { 
            _amountView.text = $"X{amount}";
        }
        _icon.sprite = _item.Icon;  
        _icon.color = new Color(1, 1, 1, 0.25f);
        _icon.sprite = _item.Icon;
    }
    public class Factory : PlaceholderFactory<HUDItemView> 
    { 
    } 
} 
