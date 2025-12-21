using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;

    private int _currentHealth;
    private bool _isDead;

    private void Awake()
    {
        // Initialize current health
        _currentHealth = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("[Enemy] Trigger with: " + other.name);
    }

    public void TakeDamage(int amount)
    {
        if (_isDead)
            return;

        _currentHealth -= amount;

        // Optional: debug log
         Debug.Log($"[EnemyHealth] {gameObject.name} took {amount} damage. HP: {_currentHealth}/{maxHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;

        // TODO: play death VFX / SFX, animation, etc.

        Destroy(gameObject);
    }
}
