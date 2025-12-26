using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 10;

    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    public event System.Action<EnemyHealth> OnEnemyDied;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;

        CurrentHealth -= amount;

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        IsDead = true;

        // Disable colliders to prevent extra hits/triggers before Destroy
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        OnEnemyDied?.Invoke(this);

        Destroy(gameObject);
    }
}

