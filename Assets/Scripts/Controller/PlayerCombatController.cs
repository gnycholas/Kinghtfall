using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<PlayerController>().OnAttack.AddListener(PerformAttack);
    }
    private void OnDisable()
    {
        GetComponent<PlayerController>().OnAttack.RemoveListener(PerformAttack);
    }

    public void PerformAttack()
    {
        
    }
}