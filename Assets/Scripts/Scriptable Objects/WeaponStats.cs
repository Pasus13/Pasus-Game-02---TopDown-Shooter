using UnityEngine;

public enum WeaponType
{
    Ranged,
    Melee
}

[CreateAssetMenu(fileName = "WeaponStats", menuName = "Stats/Weapon Stats")]
public class WeaponStats : ScriptableObject
{
    public WeaponType weaponType;

    [Header("Ranged Only")]
    public float bulletSpeed = 20f;
    public float bulletLifetime = 2f;
    public int bulletDamage = 1;
    public float fireRate = 5f;
    public bool isAutomatic = false;
    public float slowMultiplier = 0.5f; // 0.5 = 50% speed when hit
    public float slowDuration = 0.3f;   // Seconds
    public float slowRampInTime = 0.1f;      // Time to go from 1.0 to slowMultiplier
    public float slowRampOutTime = 0.2f;     // Time to go back from slowMultiplier to 1.0

    [Header("Melee Only")]
    public float swordRange = 1f;
    public float swordRangeMultiplier = 0f;
    public float swordSweepAngle = 90f;
    public float swordAttackDuration = 0.2f;
    public float swordAttackDurationMultiplier = 0f;
    public float swordCooldown = 1f;
    public int swordDamage = 5;
    public float swordKnockbackForce = 5f;       
    public float swordKnockbackDuration = 0.15f;

}

