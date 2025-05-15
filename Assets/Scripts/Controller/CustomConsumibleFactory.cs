using System.Linq;
using UnityEngine.AddressableAssets;
using Zenject;

public class CustomConsumibleFactory : IFactory<ItemSO, Consumible>
{
    [Inject] private ConsumibleRef[] _weapons;
    public Consumible Create(ItemSO param)
    {
        var selectedConsumible = _weapons.First(x => x.Id == param.Id);
        return Addressables.InstantiateAsync(selectedConsumible.Ref)
            .WaitForCompletion()
            .GetComponent<Consumible>();
    }
}
