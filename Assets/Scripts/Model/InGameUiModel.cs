using UnityEngine;

[CreateAssetMenu(menuName = "UI/InGameUiModel", fileName = "InGameUiModel")]
public class InGameUiModel : ScriptableObject
{
    [Header("Sprites dos Itens")]
    public Sprite potionSprite;
    public Sprite daggerSprite;
    public Sprite keySprite;

}
