using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce = 20f;

    private new AudioSource audio;
    private AudioSource emptyAudio;

    [Header("Ammo System")]
    public int maxAmmo = 10; // Maximum bullets per magazine
    private int currentAmmo; // Current ammo in the magazine
    public float reloadTime = 2f; // Time it takes to reload
    private bool isReloading = false; // Whether the weapon is currently reloading

    [Header("UI Elements")]
    public RectTransform ammoBarContainer; // The ammo bar's container transform
    public Image ammoBarFill; // Fill image of the ammo bar
    public ParticleSystem ammoEffect; // Particle system for ammo bar effects
    public float shakeDuration = 0.1f; // Duration of the shake effect
    public float shakeIntensity = 5f; // Intensity of the shake effect

    private Vector2 initialAmmoBarPosition; // Original anchored position of the ammo bar container

    [Header("Fire Rate System")]
    public float baseFireRate = 0.5f; // Base time between shots (in seconds)
    private float nextFireTime = 0f; // Tracks the next time the weapon can fire

    private bool isParticleSystemActive = false; // Tracks if particles have been activated

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        currentAmmo = maxAmmo; // Initialize with full ammo
        UpdateAmmoBar();

        if (ammoBarContainer != null)
        {
            // Store the initial anchored position (relative to the parent's pivot)
            initialAmmoBarPosition = ammoBarContainer.anchoredPosition;
        }
    }

    private void Update()
    {
        if (isReloading)
            return;

        // Check if player presses the reload key
        if (Input.GetKey(KeyCode.Mouse1))
        {
            StartCoroutine(Reload());
            return;
        }

        // Hold mouse button to fire
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (isReloading || currentAmmo <= 0)
        {
            if (currentAmmo <= 0)
            {

            }
            return;
        }

        float fireRateMultiplier = PlayerStats.Instance.fireRateMultiplier;
        float damageMultiplier = PlayerStats.Instance.damageMultiplier;

        // Calculate the time for the next shot using fire rate multiplier
        nextFireTime = Time.time + (baseFireRate / fireRateMultiplier);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Pass the damage multiplier to the bullet via Initialize method
        float adjustedDamage = bullet.GetComponent<Bullet>().damage * damageMultiplier;
        bullet.GetComponent<Bullet>().Initialize(adjustedDamage);

        // Add force to the bullet
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        if (bulletRigidbody != null)
        {
            bulletRigidbody.AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);
        }

        // Play audio whenever it fires
        if (audio != null)
        {
            audio.Play();
        }

        // Camera shake effect
        if (Camera.main.TryGetComponent(out CameraController cameraController))
        {
            cameraController.TriggerShake();
        }

        currentAmmo--;
        UpdateAmmoBar();
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo; // Refill ammo
        isParticleSystemActive = false; // Reset flag

        if (ammoEffect != null)
        {
            ammoEffect.Stop(); // Ensure the particle system is reset after reload
        }

        isReloading = false;

        UpdateAmmoBar();
    }

    private void UpdateAmmoBar()
    {
        if (ammoBarFill != null)
        {
            // Normalize and update the fill amount
            ammoBarFill.fillAmount = (float)currentAmmo / maxAmmo;

            // Update particle system position
            if (ammoEffect != null)
            {
                float containerHeight = ammoBarContainer.rect.height;

                // Calculate the edge position in local space (centered horizontally)
                float fillEdgeY = (ammoBarFill.fillAmount * containerHeight) - (containerHeight * 0.5f); // Adjust for pivot
                float fillEdgeX = ammoBarContainer.pivot.x * ammoBarContainer.rect.width; // Center horizontally

                // Correct for any offset in the particle system itself
                Vector3 particleOffset = new Vector3(ammoEffect.transform.localScale.x * 0.5f, 0, 0); // Adjust if necessary

                // Convert local position to world position and apply the offset
                Vector3 edgeWorldPosition = ammoBarContainer.TransformPoint(new Vector3(fillEdgeX, fillEdgeY, 0f)) + particleOffset;

                // Set the particle system's world position
                ammoEffect.transform.position = edgeWorldPosition;

                // Activate particle system only after the first shot
                if (!isParticleSystemActive && currentAmmo < maxAmmo)
                {
                    ammoEffect.Stop(); // Reset the particle system
                    ammoEffect.Play(); // Start fresh
                    isParticleSystemActive = true; // Set flag to true
                }
            }

            // Trigger shake animation
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

            // Apply the random offset to the anchored position
            ammoBarContainer.anchoredPosition = initialAmmoBarPosition + (Vector2)randomOffset;

            yield return null;
        }

        // Reset to the initial anchored position
        ammoBarContainer.anchoredPosition = initialAmmoBarPosition;
    }
}
