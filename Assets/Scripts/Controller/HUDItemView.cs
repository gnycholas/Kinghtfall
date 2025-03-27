using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HUDItemView : MonoBehaviour
{
    private static Action onSelect;
    private ItemCollectibleSO _item; 
    [SerializeField] private Image _moldure;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _amountView;

    private void Awake()
    {
        onSelect += Deselect;
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

    } 
    private void Setup(ItemCollectibleSO item, int amount)
    {
        _item = item;
        _amountView.text = $"amount";
        _icon.sprite = item.Sprite;
    }
    public class Factory : PlaceholderFactory<ItemCollectibleSO,int,HUDItemView> 
    {
        [Inject] private DiContainer _container;
        public override HUDItemView Create(ItemCollectibleSO item, int amount)
        {
            var hudItem = _container.Instantiate<HUDItemView>();
            hudItem.Setup(item, amount);
            return hudItem;
        }
    } 
} 
