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
    public int baseMaxAmmo = 10; // Changed from maxAmmo to baseMaxAmmo
    private int maxAmmo;
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

    private Vector2 mousePosition;

    private CameraController cameraShake;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        float ammoCapacityMultiplier = PlayerStats.Instance.ammoCapacityMultiplier;
        maxAmmo = Mathf.RoundToInt(baseMaxAmmo * ammoCapacityMultiplier); // Apply multiplier
        currentAmmo = maxAmmo;
        UpdateAmmoBar();

        if (ammoBarContainer != null)
        {
            initialAmmoBarPosition = ammoBarContainer.anchoredPosition;
        }

        cameraShake = Camera.main.GetComponent<CameraController>();
    }

    private void Update()
    {
        if (isReloading || currentAmmo <= 0) return;

        HandleInput();

#if !UNITY_ANDROID && !UNITY_IOS
        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(Reload());
        }
#endif
    }

    private void HandleInput()
    {
        Vector2 aimDirection;

        // Use joystick input for firing
        if (weaponJoystick.joystickDirec != Vector2.zero)
        {
            aimDirection = weaponJoystick.joystickDirec; // Use joystick direction

            if (Time.time >= nextFireTime)
            {
                Fire(aimDirection);
            }
        }

#if !UNITY_ANDROID && !UNITY_IOS
        else if (Input.GetMouseButton(0)) // Check if the left mouse button is held down
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimDirection = (mousePosition - (Vector2)firePoint.position).normalized;

            if (Time.time >= nextFireTime)
            {
                Fire(aimDirection);
            }
        }
#endif
    }

    private void Fire(Vector2 aimDirection)
    {
        if (currentAmmo <= 0 || isReloading) return;

        float fireRateMultiplier = PlayerStats.Instance.fireRateMultiplier;
        float damageMultiplier = PlayerStats.Instance.damageMultiplier;

        nextFireTime = Time.time + (baseFireRate / fireRateMultiplier);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();

        float adjustedDamage = bullet.GetComponent<Bullet>().damage * damageMultiplier;
        bullet.GetComponent<Bullet>().Initialize(adjustedDamage);

        if (bulletRigidbody != null)
        {
            bulletRigidbody.AddForce(aimDirection * fireForce, ForceMode2D.Impulse);
            RotateWeapon(aimDirection);
        }

        // Trigger camera shake
        if (cameraShake != null)
        {
            cameraShake.ShakeCameraOnce();
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
        float reloadSpeedMultiplier = PlayerStats.Instance.reloadSpeedMultiplier;
        float ammoCapacityMultiplier = PlayerStats.Instance.ammoCapacityMultiplier;

        isReloading = true;

        yield return new WaitForSeconds(reloadTime / reloadSpeedMultiplier);

        maxAmmo = Mathf.RoundToInt(baseMaxAmmo * ammoCapacityMultiplier); // Recalculate maxAmmo on reload
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
