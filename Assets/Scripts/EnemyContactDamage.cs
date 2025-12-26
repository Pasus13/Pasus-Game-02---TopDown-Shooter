using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float contactCooldown = 0.35f;

    [Header("Knockback To Player")]
    [SerializeField] private float playerKnockbackForce = 8f;
    [SerializeField] private float playerKnockbackDuration = 0.15f;

    [Header("Optional: Knockback Self On Hit")]
    [SerializeField] private bool knockbackSelf = true;
    [SerializeField] private float selfKnockbackForce = 4f;
    [SerializeField] private float selfKnockbackDuration = 0.1f;

    [Header("Layers Mask")]
    [SerializeField] private LayerMask playerLayerMask;

    private EnemyHealth _enemyHealth;
    private KnockbackReceiver _enemyKnockback;


    private float _nextDamageTime;

    private void Awake()
    {
        _enemyHealth = GetComponentInParent<EnemyHealth>();
        _enemyKnockback = GetComponentInParent<KnockbackReceiver>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_enemyHealth != null && _enemyHealth.IsDead) return;

        if (Time.time < _nextDamageTime) return;

        
        if (((1 << other.gameObject.layer) & playerLayerMask) == 0) return; // Only react to colliders that are allowed to take contact damage (player body)


        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null)
            return;

        // Apply damage
        playerHealth.TakeDamage(contactDamage);
        _nextDamageTime = Time.time + contactCooldown;

        // Apply knockback to player if possible
        IKnockbackable playerKnockback = other.GetComponentInParent<IKnockbackable>();
        if (playerKnockback != null)
        {
            Vector2 dirEnemyToPlayer = (playerHealth.transform.position - transform.position).normalized;
            playerKnockback.ApplyKnockback(dirEnemyToPlayer, playerKnockbackForce, playerKnockbackDuration);

            // Optional: knockback enemy backwards a bit
            if (knockbackSelf && _enemyKnockback != null)
            {
                _enemyKnockback.ApplyKnockback(-dirEnemyToPlayer, selfKnockbackForce, selfKnockbackDuration);
            }
        }
    }
}

