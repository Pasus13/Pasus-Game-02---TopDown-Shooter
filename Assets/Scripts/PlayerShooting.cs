using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private WeaponStats weaponStats;

    private float _fireCooldown;

    private void Update()
    {
        HandleFireCooldown();
        HandleShootingInput();
    }

    private void HandleFireCooldown()
    {
        if (_fireCooldown > 0f)
        {
            _fireCooldown -= Time.deltaTime;
        }
    }

    private void HandleShootingInput()
    {
        if (weaponStats == null)
            return;

        // If still on cooldown, cannot shoot
        if (_fireCooldown > 0f)
            return;

        bool shouldShoot = false;

        // Automatic weapon: hold button to keep shooting
        if (weaponStats.isAutomatic)
        {
            if (InputManager.ShootIsHeld)
            {
                shouldShoot = true;
            }
        }
        else
        {
            // Semi-auto: fire only when button is pressed this frame
            if (InputManager.ShootWasPressed)
            {
                shouldShoot = true;
            }
        }

        if (shouldShoot)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (firePoint == null || bulletPrefab == null)
        {
            Debug.LogWarning("[PlayerShooting] Missing firePoint or bulletPrefab.");
            return;
        }

        // Compute direction from firePoint to mouse world position
        if (Camera.main == null)
        {
            Debug.LogWarning("[PlayerShooting] No MainCamera found.");
            return;
        }

        Vector2 shootDirection = firePoint.right;

        // Instantiate bullet
        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Align bullet rotation with shoot direction (optional, for visuals)
        float bulletRotationAdjusment = -90f; // The bullets need to be rotated -90 to be alligned with the shooting direction
        float angle = (Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg) + bulletRotationAdjusment;
        bulletGO.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Initialize bullet parameters
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(shootDirection, weaponStats);
        }

        // Set fire cooldown based on fireRate (bullets/sec)
        if (weaponStats.fireRate > 0f)
        {
            _fireCooldown = 1f / weaponStats.fireRate;
        }
        else
        {
            _fireCooldown = 0.1f; // fallback
        }

        // TODO: Play shooting SFX here using your AudioManager
        // AudioManager.Instance?.PlayShoot();
    }
}

