using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    private float _currentHealth;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _injuredPercent;
    private void OnEnable()
    {
        GetComponent<PlayerController>().OnTakeDamage += ApplyDamage;
    }
    private void Start()
    {
        SetupFulllife();
    }
    private void OnDisable()
    {
        GetComponent<PlayerController>().OnTakeDamage -= ApplyDamage;
    }

    public void SetupFulllife()
    {
        _currentHealth = _maxHealth;
    }
    private DamageInfo ApplyDamage(Damage arg0)
    {
        var damage = Mathf.Max(_currentHealth - arg0.Amount, 0);
        _currentHealth = damage;
        var percent = _currentHealth / _maxHealth;
        return new DamageInfo(_currentHealth,
            percent,
            (percent > _injuredPercent)?PlayerHealthStat.NORMAL:(Mathf.Approximately(percent,0))?PlayerHealthStat.INJURED:PlayerHealthStat.INJURED,
            damage);
    }
}