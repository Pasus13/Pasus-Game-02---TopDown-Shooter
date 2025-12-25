using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    [Tooltip("Damage dealt by this sword hit.")]
    public int Damage = 5;

    // This flag tells if the hitbox is currently active for dealing damage
    public bool CanHit { get; set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!CanHit)
            return;

        Enemy enemy = other.GetComponentInParent<Enemy>();
        WeaponStats weaponStats = other.GetComponent<WeaponStats>();

        if (enemy != null)
        {
            enemy.TakeDamage(Damage);

            // Knockback
            KnockbackReceiver kb = enemy.GetComponent<KnockbackReceiver>();
            if (kb != null)
            {
                Vector2 dir = (enemy.transform.position - transform.position).normalized;
                kb.ApplyKnockback(dir, weaponStats.swordKnockbackForce, weaponStats.swordKnockbackDuration);
            }
        }

        // For now, just debug
        Debug.Log($"[SwordHitbox] Hit {other.name} for {Damage} damage.");
    }
}
