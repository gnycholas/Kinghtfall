using UnityEngine;

[CreateAssetMenu(menuName ="Prototype/Requiriment/Item")]
public sealed class ItemRequeriment : Requirement
{
    [SerializeField] private ItemSO _item;
    [SerializeField] private int _amount;
    public override bool Check(GameplayController controller)
    {
        return controller.CheckItem(_item, _amount);
    }
}
