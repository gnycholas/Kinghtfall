﻿using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName ="Prototype/Requiriment/Item")]
public sealed class ItemRequeriment : Requirement
{
    [SerializeField] private ItemSO _item;
    [SerializeField] private int _amount; 
    public override bool Check(GameplayController controller)
    {
        if (controller == null) return true;
        return controller.CheckItem(_item, _amount);
    }
}
