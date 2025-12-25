using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float invulnerabilityTime = 0.5f; // time after being hit

    private int _currentHealth;
    private bool _isDead;
    private float _invulnTimer;

    public event Action<int, int> OnHealthChanged; // current, max
    public event Action OnPlayerDied;

    private void Awake()
    {
        _currentHealth = maxHealth;

        // Notify initial health
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);
    }

    private void Update()
    {
        // Count down invulnerability timer
        if (_invulnTimer > 0f)
        {
            _invulnTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(int amount)
    {
        if (_isDead)
            return;

        // If still invulnerable, ignore damage
        if (_invulnTimer > 0f)
            return;

        _invulnTimer = invulnerabilityTime;

        _currentHealth -= amount;
        _currentHealth = Mathf.Max(_currentHealth, 0);

        OnHealthChanged?.Invoke(_currentHealth, maxHealth);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_isDead) return;

        _isDead = true;

        Debug.Log("[PlayerHealth] Player died.");

        // TODO: play death animation, SFX, show Game Over, etc.
        OnPlayerDied?.Invoke();

        // Example: disable movement / shooting
        // GetComponent<PlayerMovement>().enabled = false;
        // GetComponent<PlayerShooting>().enabled = false;
    }

    public void Heal(int amount)
    {
        if (_isDead)
            return;

        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, maxHealth);

        OnHealthChanged?.Invoke(_currentHealth, maxHealth);
    }
}

