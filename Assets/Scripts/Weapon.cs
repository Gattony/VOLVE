using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce = 20f;

    private AudioSource audio;

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

    private bool isUsingMouse = true; // Tracks whether the player is using the mouse for aiming
    private Vector2 mousePosition;

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
        Vector2 aimDirection;

        // Check if joystick is being used
        if (weaponJoystick.joystickVec != Vector2.zero)
        {
            isUsingMouse = false; // Prioritize joystick
            aimDirection = weaponJoystick.joystickVec; // Use joystick direction

            // Fire if enough time has passed
            if (Time.time >= nextFireTime)
            {
                Fire(aimDirection);
            }
        }
        else if (Input.GetMouseButton(0)) // Check if the left mouse button is held down
        {
            isUsingMouse = true; // Use mouse input
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimDirection = (mousePosition - (Vector2)firePoint.position).normalized;

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

        // Apply fire rate
        nextFireTime = Time.time + baseFireRate;

        // Spawn the bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();

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
