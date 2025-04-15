using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Model/PlayerModel", fileName = "PlayerModel")]
public class PlayerModel : ScriptableObject
{
    [Header("Configura��es de Velocidade")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
     

    [Header("Estados do Personagem")]
    public int maxHealth = 5;
    public int currentHealth = 5;  

    [Header("Invent�rio")]
    [Tooltip("Lista para armazenar itens coletados.")] 

    [Header("Estado de Hit")] 
    public float hitDuration = 1f;  
}
