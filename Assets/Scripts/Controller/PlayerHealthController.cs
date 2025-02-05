using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerView))]
public class PlayerHealthController : MonoBehaviour
{
    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private PlayerView playerView;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private AnimationClip hitAnimationClip;

    private void Awake()
    {
        if (playerModel == null)
            playerModel = Resources.Load<PlayerModel>("PlayerModel");

        if (playerView == null)
            playerView = GetComponent<PlayerView>();

        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Aplica dano ao jogador e atualiza o Model e a View.
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (playerModel.isDead)
            return;

        playerModel.ApplyDamage(damage);

        if (playerModel.isDead)
        {
            playerView.UpdateDeath(true);
            playerView.UpdateAnimations(false, false, false);
            Debug.Log("Jogador morreu!");
        }
        else
        {
            playerModel.isHit = true;
            playerView.TriggerHit();
            characterController.SimpleMove(Vector3.zero);

            float duration = (hitAnimationClip != null) ? hitAnimationClip.length : playerModel.hitDuration;
            StartCoroutine(ResetHit(duration));
        }
    }

    private IEnumerator ResetHit(float duration)
    {
        yield return new WaitForSeconds(duration);
        playerModel.isHit = false;
        playerView.ResetHitAnimation();
    }
}
