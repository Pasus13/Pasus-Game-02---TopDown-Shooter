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
    public float bulletKnockbackForce = 2f;
    public float bulletKnockbackDuration = 0.15f;

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

