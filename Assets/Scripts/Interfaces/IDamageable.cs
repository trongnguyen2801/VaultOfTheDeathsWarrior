using UnityEngine;

public interface IDamageable
{
    float MaxHealth { set; get; }
    float CurrentHealth { set; get; }
    void ApplyDamage(float dmg, Vector3 posAttack = new Vector3());
    void Die();

}