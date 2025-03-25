using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class HUDInventoryItemView : MonoBehaviour
{
    public UnityEvent<string> OnEquip;
    public UnityEvent<string> OnUnEquip;
    public string Item { get; }
    public int Amount { 
        get => _amount; set 
        {
            _amount = value;
            if(Amount == 1)
            {
                _amountView.text = "";
                return;
            }
            _amountView.text = $"X{Amount}";
        }
    }

    private string _item;
    private int _amount;
    [Inject] private ItemFactory _itemFactory;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _foreground;
    [SerializeField] private TextMeshProUGUI _amountView;

    public void Setup(string name)
    {
        _item = name;
        var data = _itemFactory.Create(name);
        _icon.color = new Color(1, 1, 1, 0.25f); 
        _icon.sprite = data.Icon;
    }
    public void Select()
    {
        _foreground.enabled = true;
    }
    public void Deselect()
    {
        _foreground.enabled = false;
    }
    public void Equip()
    {
        _icon.color = Color.white;
        OnEquip?.Invoke(_item);
    }
    public void UnEquip()
    {
        _icon.color = new Color(1,1,1,0.25f); 
        OnUnEquip?.Invoke(_item);
    }

    public class Factory : PlaceholderFactory<HUDInventoryItemView>
    {

    }
}
