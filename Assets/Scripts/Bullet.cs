using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
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
        Destroy(gameObject, _lifetime);
    }

    // Optional: handle collision here

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Example: you could check for enemy tag and deal damage
        // Then destroy the bullet
        // Destroy(gameObject);
    }
}

