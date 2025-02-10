using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce = 20f;

    private new AudioSource audio;

    [Header("Ammo System")]
    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;

    [Header("UI Elements")]
    public RectTransform ammoBarContainer;
    public Image ammoBarFill;
    public ParticleSystem ammoEffect;
    public float shakeDuration = 0.1f;
    public float shakeIntensity = 5f;

    private Vector2 initialAmmoBarPosition;

    [Header("Fire Rate System")]
    public float baseFireRate = 0.5f;
    private float nextFireTime = 0f;

    private bool isParticleSystemActive = false;

    [Header("Joystick Input")]
    public JoystickMovement weaponJoystick; // Reference to the WeaponJoystick

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoBar();

        if (ammoBarContainer != null)
        {
            initialAmmoBarPosition = ammoBarContainer.anchoredPosition;
        }
    }

    private void Update()
    {
        if (isReloading || currentAmmo <= 0) return;

        HandleInput();
    }

    private void HandleInput()
    {
        // Only use joystick input for aiming
        if (weaponJoystick.joystickDirec != Vector2.zero)
        {
            Vector2 aimDirection = weaponJoystick.joystickDirec;

            // Fire if enough time has passed
            if (Time.time >= nextFireTime)
            {
                Fire(aimDirection);
            }
        }
    }

    private void Fire(Vector2 aimDirection)
    {
        if (currentAmmo <= 0 || isReloading) return;

        float fireRateMultiplier = PlayerStats.Instance.fireRateMultiplier;
        float damageMultiplier = PlayerStats.Instance.damageMultiplier;

        // Apply fire rate
        nextFireTime = Time.time + (baseFireRate / fireRateMultiplier);

        // Spawn the bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Adding force to the bullet
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();

        // Pass the damage multiplier to the bullet via Initialize method
        float adjustedDamage = bullet.GetComponent<Bullet>().damage * damageMultiplier;
        bullet.GetComponent<Bullet>().Initialize(adjustedDamage);

        if (bulletRigidbody != null)
        {
            bulletRigidbody.AddForce(aimDirection * fireForce, ForceMode2D.Impulse);

            RotateWeapon(aimDirection);
        }

        if (audio != null)
        {
            audio.Play();
        }

        currentAmmo--;
        UpdateAmmoBar();

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }
    }

    private void RotateWeapon(Vector2 aimDirection)
    {
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, aimAngle - 90);
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isParticleSystemActive = false;

        if (ammoEffect != null)
        {
            ammoEffect.Stop();
        }

        isReloading = false;
        UpdateAmmoBar();
    }

    private void UpdateAmmoBar()
    {
        if (ammoBarFill != null)
        {
            ammoBarFill.fillAmount = (float)currentAmmo / maxAmmo;

            if (ammoEffect != null)
            {
                float containerHeight = ammoBarContainer.rect.height;
                float fillEdgeY = (ammoBarFill.fillAmount * containerHeight) - (containerHeight * 0.5f);
                float fillEdgeX = ammoBarContainer.pivot.x * ammoBarContainer.rect.width;

                Vector3 edgeWorldPosition = ammoBarContainer.TransformPoint(new Vector3(fillEdgeX, fillEdgeY, 0f));
                ammoEffect.transform.position = edgeWorldPosition;

                if (!isParticleSystemActive && currentAmmo < maxAmmo)
                {
                    ammoEffect.Stop();
                    ammoEffect.Play();
                    isParticleSystemActive = true;
                }
            }

            StartCoroutine(ShakeAmmoBar());
        }
    }

    private IEnumerator ShakeAmmoBar()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            Vector3 randomOffset = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                0f
            );

            ammoBarContainer.anchoredPosition = initialAmmoBarPosition + (Vector2)randomOffset;

            yield return null;
        }

        ammoBarContainer.anchoredPosition = initialAmmoBarPosition;
    }
}
