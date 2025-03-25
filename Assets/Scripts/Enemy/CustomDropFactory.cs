using System.Linq;
using UnityEngine;
using Zenject;

public class CustomDropFactory : IFactory<string, GameObject>
{
    [Inject(Id = "Drops")] private GameObject[] _drops;
    public GameObject Create(string param)
    {
        return GameObject.Instantiate(_drops.First(x=>x.name == param));
    }
}