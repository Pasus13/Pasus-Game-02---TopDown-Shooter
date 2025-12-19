using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private TopDownControls _controls;

    // ----- Internal state -----
    private Vector2 _movement;
    private Vector2 _mouseScreenPosition;

    private bool _shootWasPressed;
    private bool _shootIsHeld;

    private bool _swordWasPressed;
    private bool _swordIsHeld;

    private bool _dashWasPressed;

    // ----- Public static accessors -----

    public static Vector2 Movement => Instance != null ? Instance._movement : Vector2.zero;
    public static Vector2 MouseScreenPosition => Instance != null ? Instance._mouseScreenPosition : Vector2.zero;
    public static bool ShootWasPressed => Instance != null && Instance._shootWasPressed;
    public static bool ShootIsHeld => Instance != null && Instance._shootIsHeld;
    public static bool SwordWasPressed => Instance != null && Instance._swordWasPressed;
    public static bool SwordIsHeld => Instance != null && Instance._swordIsHeld;
    public static bool DashWasPressed => Instance != null && Instance._dashWasPressed;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[InputManager] Duplicate found on {gameObject.name}, destroying this one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Create the input actions instance
        _controls = new TopDownControls();
    }

    private void OnEnable()
    {
        if (_controls == null)
            _controls = new TopDownControls();

        // Enable the whole actions asset
        _controls.Enable();

        // Shortcuts for the Player action map
        var player = _controls.Player;

        // ---- Move ----
        player.Move.performed += OnMovePerformed;
        player.Move.canceled += OnMoveCanceled;

        // ---- Look (mouse position) ----
        player.Look.performed += OnLookPerformed;
        // You usually don't need canceled for mouse position, last value is fine.

        // ---- Shoot (left mouse) ----
        player.Shoot.started += OnShootStarted;
        player.Shoot.canceled += OnShootCanceled;

        // ---- Sword (right mouse) ----
        player.Sword.started += OnSwordStarted;
        player.Sword.canceled += OnSwordCanceled;

        // ---- Dash (spacebar) ----
        player.Dash.started += OnDashStarted;
    }

    private void OnDisable()
    {
        if (_controls == null) return;

        var player = _controls.Player;

        // Unsubscribe from all actions to avoid memory leaks
        player.Move.performed -= OnMovePerformed;
        player.Move.canceled -= OnMoveCanceled;

        player.Look.performed -= OnLookPerformed;

        player.Shoot.started -= OnShootStarted;
        player.Shoot.canceled -= OnShootCanceled;

        player.Sword.started -= OnSwordStarted;
        player.Sword.canceled -= OnSwordCanceled;

        player.Dash.started -= OnDashStarted;

        _controls.Disable();
    }

    private void LateUpdate()
    {
        // Reset one-frame flags at the end of the frame
        _shootWasPressed = false;
        _swordWasPressed = false;
        _dashWasPressed = false;
    }

    #region Input Callbacks

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        // Read 2D movement input (WASD / arrows)
        _movement = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        // When input is released, reset movement
        _movement = Vector2.zero;
    }

    private void OnLookPerformed(InputAction.CallbackContext ctx)
    {
        // Mouse position in screen coordinates (pixels)
        _mouseScreenPosition = ctx.ReadValue<Vector2>();
    }

    private void OnShootStarted(InputAction.CallbackContext ctx)
    {
        // Fire pressed this frame and held
        _shootWasPressed = true;
        _shootIsHeld = true;
    }

    private void OnShootCanceled(InputAction.CallbackContext ctx)
    {
        // Fire released
        _shootIsHeld = false;
    }

    private void OnSwordStarted(InputAction.CallbackContext ctx)
    {
        // Melee pressed this frame and held
        _swordWasPressed = true;
        _swordIsHeld = true;
    }

    private void OnSwordCanceled(InputAction.CallbackContext ctx)
    {
        // Melee released
        _swordIsHeld = false;
    }

    private void OnDashStarted(InputAction.CallbackContext ctx)
    {
        // Dash is usually a one-shot action, only need the "was pressed" flag
        _dashWasPressed = true;
    }

    #endregion
}

