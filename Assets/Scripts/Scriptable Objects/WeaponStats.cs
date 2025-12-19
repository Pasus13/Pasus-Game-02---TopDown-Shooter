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

    [Header("Melee Only")]
    public float swordRange = 1f;
    public float swordAttackDuration = 0.2f;
    public float swordDamage = 5f;
}

