using System.Linq;
using UnityEngine;
using Zenject;

public class CustomDropFactory : IFactory<string, GameObject>
{
    [Inject] private CollectibleItem.Factory[] _drops;
    public GameObject Create(string param)
    {
        return _drops[0].Create().gameObject;
    }
}