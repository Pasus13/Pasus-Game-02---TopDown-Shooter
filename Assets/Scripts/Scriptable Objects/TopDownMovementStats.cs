using UnityEngine;

[CreateAssetMenu(fileName = "TopDownMovementStats", menuName = "Stats/TopDown Movement Stats")]
public class TopDownMovementStats : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 6f;         // Base movement speed
    public float acceleration = 15f;      // How fast you reach target speed
    public float deceleration = 20f;      // How fast you stop when no input
    public float maxSpeed = 10f;          // Safety clamp for movement speed

    [Header("Rotation")]
    public float rotationSpeed = 720f;    // Degrees per second

    [Header("Dash")]
    public float dashSpeed = 15f;         // Dash movement speed
    public float dashDuration = 0.1f;    // How long the dash lasts in seconds
    public float dashCooldown = 1f;       // Time before you can dash again
}


