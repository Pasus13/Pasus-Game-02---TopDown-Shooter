using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    public static int EnemiesAliveCount { get; private set; }

    [Header("Reefrences")]
    [SerializeField] private WeaponStats weaponStats;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 0.5f;
    [SerializeField] private Transform _targetTransform;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;

    [Header("Damage Settings")]
    [SerializeField] private int damage = 1;


    private Rigidbody2D _rb;
    private KnockbackReceiver _enemyKnockback;
    private Coroutine _slowRoutine;     
    private Vector2 direction;

    private int _currentHealth;

    private float _baseMoveSpeed;
    private bool _isDead;
    private bool _isKnockback;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _enemyKnockback = GetComponent<KnockbackReceiver>();

        _currentHealth = maxHealth;
        _baseMoveSpeed = moveSpeed;
    }

    private void Start()
    {
        GameObject playerGO = FindAnyObjectByType<PlayerMovement>().gameObject;

        if (playerGO != null)
        {
            _targetTransform = playerGO.transform;
        }
        else
        {
            Debug.LogWarning("[EnemyChase] No GameObject with tag 'Player' found in the scene.");
        }
    }

    private void FixedUpdate()
    {
        // If enemy is under knockback, do not override velocity
        if (_enemyKnockback != null && _enemyKnockback.IsBeingKnockedBack)
            return;

        // If we don't have a target, do nothing
        if (_targetTransform == null)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        // Calculate direction to the player
        direction = (_targetTransform.position - transform.position);
        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            // Normalize to get only direction and multiply by speed
            Vector2 moveDir = direction.normalized;
            _rb.linearVelocity = moveDir * moveSpeed;

            // Optional: rotate enemy to face player
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            // If we are close enough, stop moving
            _rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnEnable()
    {
        EnemiesAliveCount++;
    }

    private void OnDisable()
    {
        EnemiesAliveCount--;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);

            // Get knockback on the player, not on the enemy
            KnockbackReceiver playerKnockback = playerHealth.GetComponent<KnockbackReceiver>();
            if (playerKnockback != null)
            {
                // Direction from enemy -> player (player gets pushed away)
                Vector2 dir = (playerHealth.transform.position - transform.position).normalized;

                playerKnockback.ApplyKnockback(dir);
                _enemyKnockback.ApplyKnockback(-dir);
            }
        }
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
