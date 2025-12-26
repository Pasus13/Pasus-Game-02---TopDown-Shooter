using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private WeaponStats _weaponStats;
    [SerializeField] private LayerMask enemyMask;

    private Rigidbody2D _rb;
    private int _damage;
    private float _lifetime;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

    }

    public void Initialize(Vector2 direction, WeaponStats weaponStats)
    {
        _damage = weaponStats.bulletDamage;
        _lifetime = weaponStats.bulletLifetime;

        // Set bullet velocity
        _rb.linearVelocity = direction.normalized * weaponStats.bulletSpeed;

        // Destroy bullet after its lifetime
        Destroy(gameObject, _lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(_damage);

            // Knockback
            IKnockbackable knockbackable = other.GetComponentInParent<IKnockbackable>();

            if (knockbackable != null)
            {
                Vector2 dir = (other.transform.position - transform.position).normalized;
                knockbackable.ApplyKnockback(dir, _weaponStats.bulletKnockbackForce, _weaponStats.bulletKnockbackDuration);
            }

            Destroy(gameObject);
        }
    }

}

