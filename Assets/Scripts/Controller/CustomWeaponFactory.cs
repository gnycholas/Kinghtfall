using System.Linq;
using UnityEngine.AddressableAssets;
using Zenject;

public class CustomWeaponFactory : IFactory<ItemSO, Weapon>
{
    [Inject] private WeaponRef[] _weapons;
    public Weapon Create(ItemSO param)
    {
        var selectedWeapon = _weapons.First(x => x.Id == param.Id);
        return Addressables.InstantiateAsync(selectedWeapon.Ref)
            .WaitForCompletion()
            .GetComponent<Weapon>();
    }
}
