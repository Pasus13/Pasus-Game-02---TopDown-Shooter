using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public TopDownMovementStats movementStats;

    [Tooltip("Optional offset if your sprite is not facing the +X direction by default.")]
    [SerializeField] private float aimAngleOffset = 0f;

    // References
    private Rigidbody2D _rb;
    private KnockbackReceiver _knockback;

    // Movement
    private Vector2 _currentVelocity;
    private Vector2 _lastAimDirection = Vector2.right;

    // Dash
    private bool _isDashing;
    private float _dashTimer;
    private float _dashCooldownTimer;
    private Vector2 _dashDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _knockback = GetComponent<KnockbackReceiver>();

        // Make sure gravity is disabled for top-down movement
        _rb.gravityScale = 0f;
    }

    private void Update()
    {
        HandleAimRotation();
        HandleDashTimers();
        HandleDashInput();
    }

    private void FixedUpdate()
    {
        // If enemy is under knockback, do not override velocity
        if (_knockback != null && _knockback.IsBeingKnockedBack)
            return;

        if (_isDashing)
        {
            ApplyDashMovement();
        }
        else
        {
            ApplyNormalMovement();
        }
    }

    #region Movement

    private void ApplyNormalMovement()
    {
        // Read movement input from InputManager (WASD / arrows)
        Vector2 moveInput = InputManager.Movement;
        Vector2 targetVelocity = moveInput * movementStats.moveSpeed;

        // Choose accel or decel depending on whether there is input
        float accel = moveInput.sqrMagnitude > 0.001f
            ? movementStats.acceleration
            : movementStats.deceleration;

        // Current velocity from Rigidbody
        _currentVelocity = _rb.linearVelocity;

        // Smoothly move from current velocity to target velocity
        _currentVelocity = Vector2.MoveTowards(
            _currentVelocity,
            targetVelocity,
            accel * Time.fixedDeltaTime
        );

        // Clamp max speed for safety
        if (_currentVelocity.magnitude > movementStats.maxSpeed)
        {
            _currentVelocity = _currentVelocity.normalized * movementStats.maxSpeed;
        }

        _rb.linearVelocity = _currentVelocity;
    }

    #endregion

    #region Aim / Rotation

    private void HandleAimRotation()
    {
        // If there is no main camera, we cannot aim
        if (Camera.main == null)
            return;

        // Read mouse position in screen space from InputManager
        Vector2 mouseScreenPos = InputManager.MouseScreenPosition;

        // Convert screen position to world position
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector2 aimDir = (mouseWorldPos - transform.position);

        // Only update aim direction if it is large enough
        if (aimDir.sqrMagnitude > 0.0001f)
        {
            _lastAimDirection = aimDir.normalized;
        }

        // Compute target angle in degrees
        float targetAngle = Mathf.Atan2(_lastAimDirection.y, _lastAimDirection.x) * Mathf.Rad2Deg;

        // Apply optional offset if your sprite is facing up instead of right, etc.
        targetAngle += aimAngleOffset;

        // Smooth rotation over time
        float currentAngle = _rb.rotation;
        float newAngle = Mathf.MoveTowardsAngle(
            currentAngle,
            targetAngle,
            movementStats.rotationSpeed * Time.deltaTime
        );

        _rb.MoveRotation(newAngle);
    }

    #endregion

    #region Dash

    private void HandleDashTimers()
    {
        // Cooldown timer
        if (_dashCooldownTimer > 0f)
        {
            _dashCooldownTimer -= Time.deltaTime;
        }

        // Dash duration timer
        if (_isDashing)
        {
            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0f)
            {
                EndDash();
            }
        }
    }

    private void HandleDashInput()
    {
        // If already dashing or on cooldown, cannot start a new dash
        if (_isDashing || _dashCooldownTimer > 0f)
            return;

        if (InputManager.DashWasPressed)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        // Prefer dash towards the aim direction (mouse)
        Vector2 dashDir = _lastAimDirection;

        // If for some reason aim direction is zero, fall back to movement direction
        if (dashDir.sqrMagnitude < 0.0001f)
        {
            Vector2 moveInput = InputManager.Movement;
            if (moveInput.sqrMagnitude > 0.0001f)
                dashDir = moveInput.normalized;
            else
                Debug.LogWarning("Player couldn´t dash");
        }

        _dashDirection = dashDir.normalized;
        _isDashing = true;
        _dashTimer = movementStats.dashDuration;
        _dashCooldownTimer = movementStats.dashCooldown;

        // Here you can enable invulnerability, trail effects, etc.
        // Example: _isInvulnerable = true;
    }

    private void ApplyDashMovement()
    {
        // During dash, override normal movement with a fixed dash velocity
        _rb.linearVelocity = _dashDirection * movementStats.dashSpeed;
    }

    private void EndDash()
    {
        _isDashing = false;
        // After dash ends, you can disable invulnerability or effects here
        // Example: _isInvulnerable = false;
    }

    #endregion
}

