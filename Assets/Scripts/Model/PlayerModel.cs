using UnityEngine;

[CreateAssetMenu(menuName = "Model/PlayerModel", fileName = "PlayerModel")]
public class PlayerModel : ScriptableObject
{
    [Header("Configurações de Velocidade")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;

    [Header("Estados de Movimento")]
    public bool isWalking;
    public bool isRunning;

    [Header("Estados do Personagem")]
    public int maxHealth = 5;
    public int currentHealth = 5;
    public bool isInjured;
    public bool isDead;
    public bool isKnifeEquiped;

    [Header("Estado de Hit")]
    public bool isHit;
    // Valor padrão (caso o clip não seja atribuído); será substituído pelo comprimento do clip de hit.
    public float hitDuration = 1f;
}
