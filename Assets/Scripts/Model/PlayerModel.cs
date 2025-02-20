using System.Collections.Generic;
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
    public bool isTurningLeft;
    public bool isTurningRight;
    public bool isBacking;

    [Header("Estados do Personagem")]
    public int maxHealth = 5;
    public int currentHealth = 5;
    public bool isInjured;
    public bool isDead;
    public bool isKnifeEquipped;

    [Header("Inventário")]
    [Tooltip("Lista para armazenar itens coletados.")]
    public List<GameObject> inventory = new List<GameObject>();
    public bool isPotionEquipped;
    public bool isKeyEquipped; // Flag para a chave

    [Header("Estado de Hit")]
    public bool isHit;
    public float hitDuration = 1f;

    [Header("Estado de Ataque")]
    public bool isAttacking;

    [Header("Estado de Consumir Poção")]
    public bool isDrinking;

    /// <summary>
    /// Atualiza a vida do player ao sofrer dano.
    /// </summary>
    public void ApplyDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        isInjured = (currentHealth < 3 && currentHealth > 0);
        isDead = (currentHealth <= 0);
    }
}
