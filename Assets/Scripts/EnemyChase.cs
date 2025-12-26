using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; // Optional override
    [SerializeField] private string targetTag = "Player";

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 0.5f;
    [SerializeField] private bool rotateToFaceTarget = true;

    private Rigidbody2D _rb;
    private EnemyHealth _health;
    private KnockbackReceiver _knockback;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<EnemyHealth>();
        _knockback = GetComponent<KnockbackReceiver>();
    }

    private void Start()
    {
        if (target == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag(targetTag);
            if (go != null) target = go.transform;
        }

        if (target == null)
            Debug.LogWarning("[EnemyChase] No target found. Assign it or tag Player.");
    }

    private void FixedUpdate()
    {
        if (_health != null && _health.IsDead) return;

        // If under knockback, don't override velocity
        if (_knockback != null && _knockback.IsBeingKnockedBack)
            return;

        if (target == null)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 toTarget = (Vector2)(target.position - transform.position);
        float dist = toTarget.magnitude;

        if (dist > stopDistance)
        {
            Vector2 dir = toTarget.normalized;
            _rb.linearVelocity = dir * moveSpeed;

            if (rotateToFaceTarget)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                _rb.MoveRotation(angle);
            }
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }
}

