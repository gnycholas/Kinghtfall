using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName ="Prototype/Item")]
public class ItemCollectibleSO : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private AnimatorOverrideController _animator;
    [SerializeField] private float _time;
    [SerializeField] private AssetReferenceSprite _icon;
    public string Name { get => _name;}
    public AnimatorOverrideController Animator { get => _animator;}
    public float Time { get => _time; }
    public Sprite Sprite { get => Addressables.LoadAssetAsync<Sprite>(_icon).WaitForCompletion(); }
}
