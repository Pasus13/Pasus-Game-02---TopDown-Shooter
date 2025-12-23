using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    [Header("Reefrences")]
    [SerializeField] private WeaponStats weaponStats;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 0.5f;
    [SerializeField] private Transform _targetTransform;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;

    private Rigidbody2D _rb;
    private Coroutine _slowRoutine;     
    private Coroutine _knockbackRoutine;
    private Vector2 direction;

    private int _currentHealth;
    private float _baseMoveSpeed;
    private bool _isDead;
    private bool _isKnockback;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

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
        // If we are currently in knockback, do not chase the player
        if (_isKnockback)
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

    public void ApplyHitSlow()
    {
        // Stop previous slow coroutine if it is still running
        if (_slowRoutine != null)
        {
            StopCoroutine(_slowRoutine);
        }

        _slowRoutine = StartCoroutine(HitSlowCoroutine());
    }

    public void ApplyKnockback()
    {
        if (_knockbackRoutine != null)
            StopCoroutine(_knockbackRoutine);

        _knockbackRoutine = StartCoroutine(KnockbackCoroutine());
    }

    private IEnumerator HitSlowCoroutine()
    {
        float finalFactor = Mathf.Clamp01(weaponStats.slowMultiplier);
        float startFactor = 1f;

        // --- RAMP IN: 1.0 -> finalFactor ---
        if (weaponStats.slowRampInTime > 0f)
        {
            float t = 0f;
            while (t < weaponStats.slowRampInTime)
            {
                float lerp = t / weaponStats.slowRampInTime;
                float factor = Mathf.Lerp(startFactor, finalFactor, lerp);
                moveSpeed = _baseMoveSpeed * factor;

                t += Time.deltaTime;
                yield return null;
            }
        }

        // Ensure we end exactly at finalFactor
        moveSpeed = _baseMoveSpeed * finalFactor;

        // --- HOLD FULL SLOW ---
        if (weaponStats.slowDuration > 0f)
        {
            float timer = 0f;
            while (timer < weaponStats.slowDuration)
            {
                // Keep speed constant at the slow factor
                moveSpeed = _baseMoveSpeed * finalFactor;
                timer += Time.deltaTime;
                yield return null;
            }
        }

        // --- RAMP OUT: finalFactor -> 1.0 ---
        if (weaponStats.slowRampOutTime > 0f)
        {
            float t = 0f;
            while (t < weaponStats.slowRampOutTime)
            {
                float lerp = t / weaponStats.slowRampOutTime;
                float factor = Mathf.Lerp(finalFactor, startFactor, lerp);
                moveSpeed = _baseMoveSpeed * factor;

                t += Time.deltaTime;
                yield return null;
            }
        }

        // Ensure we end exactly at base speed
        moveSpeed = _baseMoveSpeed;
        _slowRoutine = null;
    }

    private IEnumerator KnockbackCoroutine()
    {
        _isKnockback = true;

        // Normalize direction and scale by force
        Vector2 knockbackVelocity = -1 * direction.normalized * weaponStats.swordKnockbackForce;

        float timer = 0f;

        while (timer < weaponStats.swordKnockbackDuration)
        {
            _rb.linearVelocity = knockbackVelocity;
            timer += Time.deltaTime;
            yield return null;
        }

        // Stop knockback and resume normal movement
        _isKnockback = false;
        _knockbackRoutine = null;
    }
}
