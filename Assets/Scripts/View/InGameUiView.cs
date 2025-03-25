using UnityEngine; 
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Linq;
using UnityEngine.InputSystem;
using System;
using Zenject;

public class InGameUiView : MonoBehaviour
{
    [Header("Referências ao Painel de Inventário")]
    [SerializeField] private Transform itensGridPanel;
    [Inject] private HUDInventoryItemView.Factory _itemViewFactory;
    private GameInputs _inputs;
    // Este prefab pode ser só uma Image (UnityEngine.UI.Image) com layout configurado.

    // Para armazenar dinamicamente as imagens criadas
    private LinkedList<HUDInventoryItemView> _listItems = new();
    private LinkedListNode<HUDInventoryItemView> _selectedItem;

    private void Awake()
    {
        _inputs = new GameInputs();
        _inputs.Gameplay.Enable();
    }
    private void OnEnable()
    {
        _inputs.Gameplay.SelectItem.started += SelectItem;
    }
    private void OnDisable()
    { 
        _inputs.Gameplay.SelectItem.started -= SelectItem;
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

    /// <summary>
    /// Cria (ou reativa) a Image no grid. Ajusta o sprite e alpha.
    /// </summary>
    public void ShowItem(string itemKey)
    {
        var item = _listItems.FirstOrDefault(x => x.Item == itemKey);
        if (item == default)
        {
            // Instancia uma nova Image (ou outro objeto UI).
            HUDInventoryItemView itemView = _itemViewFactory.Create();
            itemView.Setup(itemKey);
            itemView.transform.SetParent(itensGridPanel); 
            
            _listItems.AddLast(itemView);
            if(_listItems.Count == 1)
            {
                _selectedItem = _listItems.First;
                _selectedItem.Value.Select();
            }
        }
        else
        {
            item.Amount++;
        } 
    }

    /// <summary>
    /// Esconde (ou remove) o item do grid
    /// </summary>
    public void HideItem(string itemKey)
    {
        var item = _listItems.FirstOrDefault(x => x.Item == itemKey);
        if (item == default)
        {
            return;
        }
        else
        {
            _listItems.Remove(item);
            Addressables.ReleaseInstance(item.gameObject);
        }
    }

    public void NextItem()
    { 
        if(_selectedItem.Next != null)
        {
            _selectedItem.Value.Deselect();
            _selectedItem = _selectedItem.Next;
            _selectedItem.Value.Select();
        }
        else
        {
            _selectedItem = _listItems.First;
        }
    }
    public void PreviusItem()
    {
        if (_selectedItem.Previous != null)
        {
            _selectedItem.Value.Deselect();
            _selectedItem = _selectedItem.Previous;
            _selectedItem.Value.Select();
        }
        else
        {
            _selectedItem = _listItems.Last;
        }
    }
}
