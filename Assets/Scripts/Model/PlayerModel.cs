using UnityEngine;

/// <summary>
/// Armazena dados do jogador, como velocidades, estados, etc.
/// Não deve conter lógica de Engine (como transform.Translate).
/// </summary>
[CreateAssetMenu(menuName = "Model/PlayerModel", fileName = "PlayerModel")]
public class PlayerModel : ScriptableObject
{
    [Header("Configurações de Velocidade")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;

    [Header("Estados")]
    public bool isWalking;
    public bool isRunning;

    // Se precisar de mais estados, acrescente aqui
    // como ex.: health, stamina, etc.
}
