using UnityEngine; 
using System.Collections.Generic; 
using UnityEngine.InputSystem; 
using Zenject;
using System.Linq;
using System;

public class GameplayUIView : MonoBehaviour
{
    [Header("Referências ao Painel de Inventário")]
    [SerializeField] private Transform itensGridPanel; 
    [Inject] private HUDItemView.Factory _itemViewFactory;
    [Inject] private InventoryController _inventoryController;  
    private LinkedList<HUDItemView> _items = new();
    private LinkedListNode<HUDItemView> _selectedItem;
    private GameInputs _inputs;

    private void Awake()
    {
        _inputs = new GameInputs();
        _inputs.Gameplay.Enable();
    }
    private void OnEnable()
    {
        _inputs.Gameplay.SelectItem.started += SelectItem;
        _inputs.Gameplay.Equip.started += Equip;
        _inventoryController.OnUpdateInventory.AddListener(UpdateInventory);
    }
     
    private void OnDisable()
    { 
        _inputs.Gameplay.SelectItem.started -= SelectItem;
        _inputs.Gameplay.Equip.started -= Equip;
        _inventoryController.OnUpdateInventory.RemoveListener(UpdateInventory);
    }

    private void Equip(InputAction.CallbackContext context)
    {
        if (_selectedItem.Value.Equiped)
        {
            _selectedItem.Value.UnEquip(_inventoryController);
        }
        else
        {
            _selectedItem.Value.Equip(_inventoryController); 
        }
    }

    private void UpdateInventory(ItemInventory item)
    {
        HUDItemView selected = _items.FirstOrDefault(x => x.Position == item.Position);

        if (selected != null)
        {
            selected.Amount = item.Amount;
            if (selected.Amount == 0)
            { 
                Destroy(selected.gameObject);
                _items.Remove(selected);
            }
        }
        else
        { 
            var HUDItem = _itemViewFactory.Create();
            if(_items.Count > 0)
            { 
                var list = _items.ToList();
                list.Add(HUDItem);
                HUDItem.Setup(item.Info.Id, item.Amount, item.Position);
                _items = new LinkedList<HUDItemView>(list.OrderBy(x => x.Position));
                HUDItem.transform.SetParent(itensGridPanel);
            }
            else
            {
                HUDItem.Setup(item.Info.Id, item.Amount, item.Position);
                _items.AddLast(HUDItem);
                _selectedItem = _items.First;
                _selectedItem.Value.Select();
                HUDItem.transform.SetParent(itensGridPanel);
            }
        } 
    }

    private void SelectItem(InputAction.CallbackContext context)
    {
        if (_selectedItem == null)
            return;
        var value = context.ReadValue<float>();
        if(value > 0)
        {
            NextItem();
        }
        else
        {
            PreviusItem();
        }
    }

     
    public void NextItem()
    {  
        if(_selectedItem.Next != null)
        { 
            _selectedItem = _selectedItem.Next; 
        }
        else
        {
            _selectedItem = _items.First;
        } 
        _selectedItem.Value.Select();
    }
    public void PreviusItem()
    {
        if (_selectedItem.Previous != null)
        { 
            _selectedItem = _selectedItem.Previous; 
        }
        else
        {
            _selectedItem = _items.Last;
        } 
        _selectedItem.Value.Select();
    }
}
