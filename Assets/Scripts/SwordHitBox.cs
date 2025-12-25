using UnityEngine;
using System;
using System.Collections.Generic;

public class SwordHitbox : MonoBehaviour
{
    [SerializeField] private WeaponStats weaponStats;

    [Tooltip("Damage dealt by this sword hit.")]
    public int Damage;

    public bool CanHit { get; set; }

    private readonly HashSet<Enemy> _hitThisSwing = new HashSet<Enemy>();

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

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        // Prevent multiple hits during the same swing
        if (_hitThisSwing.Contains(enemy)) return;

        _hitThisSwing.Add(enemy);
        enemy.TakeDamage(weaponStats.swordDamage);

        // Knockback
        IKnockbackable knockbackable = enemy.GetComponent<IKnockbackable>();
        if (knockbackable != null)
        {
            Vector2 dir = (enemy.transform.position - transform.position).normalized;
            knockbackable.ApplyKnockback(dir, weaponStats.swordKnockbackForce, weaponStats.swordKnockbackDuration);
        }
    }
}
