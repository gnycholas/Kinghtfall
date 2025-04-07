using UnityEngine;

[CreateAssetMenu(menuName = "UI/InGameUiModel", fileName = "InGameUiModel")]
public class InGameUiModel : ScriptableObject
{
    [Header("Sprites dos Itens")]
    public Sprite potionSprite;
    public Sprite daggerSprite;
    public Sprite keySprite;

    [Header("Bools de verifica��o")]
    //Essas bools come�am falsas e ficam verdadeiras apenas depois a primeira vez que a a��o � feita e o tutorial � mostrado
    public bool hasStarted = false; 
    public bool collectedDagger = false;
    public bool collectedPotion = false;
}
