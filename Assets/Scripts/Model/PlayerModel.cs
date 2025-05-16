using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Model/PlayerModel", fileName = "PlayerModel")]
public class PlayerModel : ScriptableObject
{
    [Header("Configurações de Velocidade")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
     

    [Header("Estados do Personagem")]
    public int maxHealth = 5;
    public int currentHealth = 5;  

    [Header("Inventário")]
    [Tooltip("Lista para armazenar itens coletados.")] 

    [Header("Estado de Hit")] 
    public float hitDuration = 1f;  
}
