using System;
using System.Collections.Generic; 

public class Inventory 
{
    private Dictionary<int, Item> _dictionaryItem = new(); 
    public Inventory()
    {
        for(int index = 0; index < 10; index++)
        {
            _dictionaryItem[index] = default;
        }
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
