using System.Collections;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponStats weaponStats;
    [SerializeField] private Transform swordSlashTransform; 
    [SerializeField] private ParticleSystem swordSlashVFX;
    [SerializeField] private Transform playerTransform;  

    private SwordHitbox _swordHitbox;
    private Vector3 _baseSlashScale;
    private Quaternion _idleLocalRotation;
    private bool _isAttacking;
    private float _cooldownTimer;
    

    private void Awake()
    {
        if (swordSlashTransform != null)
        {
            _swordHitbox = swordSlashTransform.GetComponent<SwordHitbox>();
            _baseSlashScale = swordSlashTransform.localScale;
            _idleLocalRotation = swordSlashTransform.localRotation;

            // Ensure the slash starts disabled
            swordSlashTransform.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[PlayerMeleeAttack] SwordSlashTransform is not assigned.");
        }
    }

    private void Update()
    {
        HandleCooldown();
        HandleInput();
    }

    private void HandleCooldown()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
    }

    private void HandleInput()
    {
        if (weaponStats == null || swordSlashTransform == null)
            return;

        if (_isAttacking)
            return;

        if (_cooldownTimer > 0f)
            return;

        // Right mouse button pressed (Sword attack)
        if (InputManager.SwordWasPressed)
        {
            StartCoroutine(SwordAttackCoroutine());
        }
    }

    private IEnumerator SwordAttackCoroutine()
    {
        _isAttacking = true;

        // --- COOLDOWN SETUP ---
        _cooldownTimer = weaponStats.swordCooldown;

        // --- RANGE SETUP ---
        float rangeMultiplier = Mathf.Max(weaponStats.swordRangeMultiplier, 0.01f);
        Vector3 scaled = _baseSlashScale;
        // We rescale sword and slash efect base on rangeMultiplayer
        scaled.x = _baseSlashScale.x * rangeMultiplier;

        swordSlashTransform.localScale = scaled;
        swordSlashVFX.transform.localScale = scaled;

        // --- DAMAGE / HITBOX SETUP ---
        if (_swordHitbox != null)
        {
            _swordHitbox.Damage = weaponStats.swordDamage;
            _swordHitbox.BeginSwing();
        }

        // --- SWEEP PARAMETERS ---
        float totalAngle = weaponStats.swordSweepAngle;
        float halfAngle = totalAngle * 0.5f;

        float baseDuration = Mathf.Max(weaponStats.swordAttackDuration, 0.01f);

        // Treat swordAttackDurationMultiplier as "speed": 1 = normal, 2 = twice as fast
        float speedMultiplier = Mathf.Max(weaponStats.swordAttackDurationMultiplier, 0.01f);
        float duration = baseDuration / speedMultiplier;

        // Store base WORLD rotation at attack start (so the slash direction is locked)
        Quaternion baseWorldRot = swordSlashTransform.rotation;

        // Enable visual + collider object
        swordSlashTransform.gameObject.SetActive(true);

        if (swordSlashVFX != null)
        {
            swordSlashVFX.Clear(); // ensures no old particles
            swordSlashVFX.Play();
        }

        float elapsed = 0f;

        // --- SWING LOOP ---
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float angleOffset = Mathf.Lerp(-halfAngle, +halfAngle, t);

            swordSlashTransform.rotation = baseWorldRot * Quaternion.Euler(0f, 0f, angleOffset);

            elapsed += Time.deltaTime;
            yield return null;
        }

        swordSlashTransform.rotation = playerTransform.rotation;

        // --- DISABLE HITBOX ---
        if (_swordHitbox != null)
        {
            _swordHitbox.EndSwing();
        }

        // --- DISABLE VISUAL & RESET TRANSFORM ---
        swordSlashTransform.gameObject.SetActive(false);
        swordSlashTransform.localScale = _baseSlashScale;
        // We do not force localRotation here, world rotation is already reset above.

        _isAttacking = false;
    }
}

