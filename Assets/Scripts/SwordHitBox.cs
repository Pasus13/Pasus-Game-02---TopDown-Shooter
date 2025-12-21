using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    [Tooltip("Damage dealt by this sword hit.")]
    public int damage = 1;

    // This flag tells if the hitbox is currently active for dealing damage
    public bool CanHit { get; set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!CanHit)
            return;

        // Example: look for an enemy health component
        // var enemy = other.GetComponent<EnemyHealth>();
        // if (enemy != null)
        // {
        //     enemy.TakeDamage(damage);
        // }

        // For now, just debug
        Debug.Log($"[SwordHitbox] Hit {other.name} for {damage} damage.");
    }
}
