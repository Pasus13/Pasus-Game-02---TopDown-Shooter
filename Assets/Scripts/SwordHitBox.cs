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
        if (enemy != null)
        {
            enemy.TakeDamage(Damage);
            enemy.ApplyKnockback();
        }

        // For now, just debug
        Debug.Log($"[SwordHitbox] Hit {other.name} for {Damage} damage.");
    }
}
