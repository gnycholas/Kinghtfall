using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;


public class CollectibleItem : MonoBehaviour,IInteract
{
    [SerializeField] private ItemSO _item;
    [SerializeField] private int _amount;
    [Inject] private GameplayController _controller;

    public async Task Execute()
    {
        await _controller.AddItemToInventory(_item, _amount);
        await UniTask.CompletedTask;
        Destroy(gameObject);
    }

    public AnimatorOverrideController GetInteraction()
    {
        return null;
    }

    public Transform GetTarget()
    {
        return null;
    }

    public class Factory : PlaceholderFactory<CollectibleItem>
    {

    }
}
