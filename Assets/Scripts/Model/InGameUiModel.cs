using UnityEngine;

[CreateAssetMenu(menuName = "UI/InGameUiModel", fileName = "InGameUiModel")]
public class InGameUiModel : ScriptableObject
{
    [Header("Sprites dos Itens")]
    public Sprite potionSprite;
    public Sprite daggerSprite;
    public Sprite keySprite;

    [Header("Bools de verificação")]
    //Essas bools começam falsas e ficam verdadeiras apenas depois a primeira vez que a ação é feita e o tutorial é mostrado
    public bool hasStarted = false; 
    public bool collectedDagger = false;
    public bool collectedPotion = false;
}
