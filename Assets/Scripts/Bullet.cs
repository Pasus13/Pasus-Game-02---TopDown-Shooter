using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private WeaponStats _weaponStats;

    private Rigidbody2D _rb;
    private int _damage;
    private float _lifetime;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

    }

    public void Initialize(Vector2 direction, float speed, float lifetime, int damage)
    {
        _damage = damage;
        _lifetime = lifetime;

        // Set bullet velocity
        _rb.linearVelocity = direction.normalized * speed;

        // Destroy bullet after its lifetime
        //Destroy(gameObject, _lifetime);
        Destroy(gameObject, 10);
    }

    // Optional: handle collision here

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(_damage);
            //enemy.ApplyHitSlow();

            // Knockback
            KnockbackReceiver kb = enemy.GetComponent<KnockbackReceiver>();

            if (kb != null)
            {
                Vector2 dir = (enemy.transform.position - transform.position).normalized;
                kb.ApplyKnockback(dir, _weaponStats.bulletKnockbackForce, _weaponStats.bulletKnockbackDuration);
            }

            Destroy(gameObject);
        }
    }

}

