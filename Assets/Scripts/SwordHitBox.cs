using UnityEngine;
using System;
using System.Collections.Generic;

public class SwordHitbox : MonoBehaviour
{
    [SerializeField] private WeaponStats weaponStats;

    [Tooltip("Damage dealt by this sword hit.")]
    public int Damage;

    public bool CanHit { get; set; }

    private readonly HashSet<IDamageable> _hitThisSwing = new HashSet<IDamageable>();

    // Call this at the start of each sword attack
    public void BeginSwing()
    {
        _hitThisSwing.Clear();
        CanHit = true;
    }

    // Call this at the end of each sword attack
    public void EndSwing()
    {
        CanHit = false;
        _hitThisSwing.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!CanHit) return;

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy == null) return;

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable == null) return;

        // Prevent multiple hits during the same swing
        if (_hitThisSwing.Contains(damageable)) return;

        _hitThisSwing.Add(damageable);

        damageable.TakeDamage(weaponStats.swordDamage);

        // Knockback
        IKnockbackable knockbackable = other.GetComponentInParent<IKnockbackable>();
        if (knockbackable != null)
        {
            Vector2 dir = (other.transform.position - transform.position).normalized;
            knockbackable.ApplyKnockback(dir, weaponStats.swordKnockbackForce, weaponStats.swordKnockbackDuration);
        }
    }
}
