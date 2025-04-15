using Zenject;
using System.Linq;

public class CustomItemFactory : IFactory<string, ItemSO>
{
    [Inject] private ItemSO[] _items;
    public ItemSO Create(string param)
    {
        return _items.First(x=>x.Id == param);
    }
}
