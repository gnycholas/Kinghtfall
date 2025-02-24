using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Refer�ncia ao Animator")]
    [SerializeField] private Animator animator;

    [Header("Refer�ncia aos Audio Sources")]
    [SerializeField] private AudioSource walkingAudioSource;
    [SerializeField] private AudioClip[] walkingAudioClips;
    [SerializeField] private AudioSource runningAudioSource;
    [SerializeField] private AudioClip[] runningAudioClips;
    [SerializeField] private AudioSource attackingAudioSource;
    [SerializeField] private AudioClip[] attackingAudioClips;
    [SerializeField] private AudioSource dyingAudioSource;
    [SerializeField] private AudioClip[] dyingAudioClips;
    [SerializeField] private AudioSource hitAudioSource;
    [SerializeField] private AudioClip[] hitAudioClips;

    [Header("Refer�ncia aos Objetos Visuais")]
    [SerializeField] private GameObject knifeGameObject;
    [SerializeField] private GameObject potionGameObject; // Objeto visual da po��o (na m�o quando equipada)
    [SerializeField] private GameObject keyGameObject;    // Objeto visual da chave (na m�o quando equipada)

    private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");
    private static readonly int HitTriggerHash = Animator.StringToHash("Hit");

    public void UpdateAnimations(bool isWalking, bool isRunning, bool isInjured)
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isInjured", isInjured);
    }

    public void TriggerHit()
    {
        animator.SetTrigger(HitTriggerHash);
        animator.SetBool("isHit", true);
    }

    public void ResetHitAnimation()
    {
        animator.SetBool("isHit", false);
    }

    public void UpdateDeath(bool isDead)
    {
        animator.SetBool("isDead", isDead);
    }

    public void UpdateKnifeEquip(bool isKnifeEquipped)
    {
        animator.SetBool("isKnifeEquipped", isKnifeEquipped);
        if (knifeGameObject != null)
            knifeGameObject.SetActive(isKnifeEquipped);
    }

    public void TriggerAttack()
    {
        animator.SetTrigger(AttackTriggerHash);
    }

    public void SetAttacking(bool attacking)
    {
        animator.SetBool("isAttacking", attacking);
    }

    public void UpdatePotionEquip(bool isEquipped)
    {
        animator.SetBool("isPotionEquipped", isEquipped);
        if (potionGameObject != null)
            potionGameObject.SetActive(isEquipped);
    }

    public void TriggerPotionDrink()
    {
        animator.SetTrigger("PotionDrink");
    }

    public void UpdateDrinking(bool isDrinking)
    {
        animator.SetBool("isDrinking", isDrinking);
    }

    public void UpdateKeyEquip(bool isEquipped)
    {
        if (keyGameObject != null)
            keyGameObject.SetActive(isEquipped);
    }

    // Atualiza os estados de giro (virada) � para acionar anima��es Left_Turn, Right_Turn, Walking_Backward
    public void UpdateTurning(bool isTurningLeft, bool isTurningRight, bool isBacking)
    {
        animator.SetBool("isTurningLeft", isTurningLeft);
        animator.SetBool("isTurningRight", isTurningRight);
        animator.SetBool("isBacking", isBacking);
    }

    // Atualiza o estado de coleta (catching)
    public void UpdateCatching(bool isCatching)
    {
        animator.SetBool("isCatching", isCatching);
    }

    private void PassoSoundEvent()
    {
        int index = Random.Range(0, walkingAudioClips.Length);
        walkingAudioSource.PlayOneShot(walkingAudioClips[index]);
    }

    private void CorrerSoundEvent()
    {
        int index = Random.Range(0, runningAudioClips.Length);
        runningAudioSource.PlayOneShot(runningAudioClips[index]);
    }

    private void AttackSoundEvent()
    {
        int index = Random.Range(0, attackingAudioClips.Length);
        attackingAudioSource.PlayOneShot(attackingAudioClips[index]);
    }

    private void DyingSoundEvent()
    {
        int index = Random.Range(0, dyingAudioClips.Length);
        dyingAudioSource.PlayOneShot(dyingAudioClips[index]);
    }

    private void HitSoundEvent()
    {
        int index = Random.Range(0, hitAudioClips.Length);
        hitAudioSource.PlayOneShot(hitAudioClips[index]);
    }

}
