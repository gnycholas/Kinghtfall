using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private Transform _slotHandLeft;
    [SerializeField] private Transform _slotHandRight;
    [Inject] private WeaponFactory _weaponFactory;
    private void OnEnable()
    {
        GetComponent<PlayerController>().OnAttack.AddListener(PerformAttack);
        GetComponent<PlayerController>().OnEquipWeapon.AddListener(EquipWeapon);
    }
     

    private void OnDisable()
    {
        GetComponent<PlayerController>().OnAttack.RemoveListener(PerformAttack);
        GetComponent<PlayerController>().OnEquipWeapon.RemoveListener(EquipWeapon); 
    }

    private void EquipWeapon(Item weapon)
    {
        WeaponView weaponView = _weaponFactory.Create(weapon);
        if(weaponView.WeaponGrip == WeaponGrip.OneHand_Left)
        {
            weaponView.transform.SetParent(_slotHandLeft); 
        }
        else if (weaponView.WeaponGrip == WeaponGrip.OneHand_Right)
        {
            weaponView.transform.SetParent(_slotHandRight);
        }
        weaponView.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0,0,0));
    }
    public void PerformAttack()
    {
        
    }
}
public class WeaponFactory : PlaceholderFactory<Item, WeaponView>
{

}
public class CustomWeaponFactory : IFactory<Item, WeaponView>
{
    [Inject] private WeaponRef[] _weapons;
    public WeaponView Create(Item param)
    {
        return Addressables.InstantiateAsync(_weapons.First(x => x.Name == param.Name).Ref).WaitForCompletion().GetComponent<WeaponView>();
    }
}
public enum WeaponGrip
{
    OneHand_Left,
    OneHand_Right,
}