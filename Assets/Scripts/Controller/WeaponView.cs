using UnityEngine;

public sealed class WeaponView : MonoBehaviour
{
    public float DamageBase;
    protected GameObject owner;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (owner == null)
            return;
        if(collision.gameObject.TryGetComponent(out ITarget target) && !target.Equals(owner))
        {
            target.TakeDamage(new Damage(DamageBase));
        }
    }

    public void SetOwner(GameObject owner)
    {
        GetComponent<Collider>().isTrigger = false;
        this.owner = owner;
    }
}
