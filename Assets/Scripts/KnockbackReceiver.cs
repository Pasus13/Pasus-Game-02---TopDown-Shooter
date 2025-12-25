using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockbackReceiver : MonoBehaviour
{
    [Header("Knockback Settings")]
    [SerializeField] private float defaultForce;
    [SerializeField] private float defaultDuration;

    [Header("References")]
    [SerializeField] private AnimationCurve recoveryCurve;
    [SerializeField] private WeaponStats weaponStats;

    private Rigidbody2D _rb;
    private Coroutine _knockCoroutine;
    
    private bool _isBeingKnockedBack;

    public bool IsBeingKnockedBack => _isBeingKnockedBack;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (recoveryCurve == null || recoveryCurve.length == 0)
        {
            // Default: linear decay from 1 to 0
            recoveryCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        }

        defaultForce = weaponStats.swordKnockbackForce;
        defaultDuration = weaponStats.swordKnockbackDuration;
    }


    /// direction: Direction of the knockback (will be normalized).
    /// forceOverride If <= 0, uses defaultForce.
    /// durationOverride If <= 0, uses defaultDuration.
    public void ApplyKnockback(Vector2 direction, float forceOverride = -1f, float durationOverride = -1f)
    {
        if (_rb == null)
            return;

        float force = (forceOverride > 0f) ? forceOverride : defaultForce;
        float duration = (durationOverride > 0f) ? durationOverride : defaultDuration;

        direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.zero;

        if (_knockCoroutine != null)
        {
            StopCoroutine(_knockCoroutine);
        }

        _knockCoroutine = StartCoroutine(KnockbackRoutine(direction * force, duration));
    }

    private IEnumerator KnockbackRoutine(Vector2 knockVelocity, float duration)
    {
        _isBeingKnockedBack = true;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            float factor = recoveryCurve.Evaluate(t);

            // Apply decaying knockback velocity
            _rb.linearVelocity = knockVelocity * factor;

            elapsed += Time.deltaTime;
            yield return null;
        }

        _isBeingKnockedBack = false;
    }
}

