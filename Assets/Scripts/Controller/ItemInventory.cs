using System;

public record ItemInventory :IEquatable<string>
{
    public ItemSO Info;
    public int Amount;
    public int Position;
    public ItemType Type => Info.ItemType;

    public ItemInventory(ItemSO info, int amount,int position)
    {
        Info = info;
        Amount = amount; 
        Position = position; 
    }

    public bool Equals(string other)
    {
        return Info.Id == other;
    }
    public enum ItemType
    {
        CONSUMIBLE,
        EQUIPMENT
    }
}
