[System.Serializable]
public class ItemInfo
{
    public string Name;
    public int Amount;  
    public ItemType Type;

    public Item CachedToItem()
    {
        return new Item() { Amount = Amount, Name = Name, Type = Type };
    }
}
