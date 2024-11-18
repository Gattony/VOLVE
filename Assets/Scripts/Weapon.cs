using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce = 20f;

    private new AudioSource audio;

    // Ammo and reloading variables
    public int maxAmmo = 10;           // Maximum bullets per magazine
    private int currentAmmo;           // Current ammo in the magazine
    public float reloadTime = 2f;      // Time it takes to reload
    private bool isReloading = false;  // Whether the weapon is currently reloading

    // Fire rate and damage multipliers from PlayerStats
    public float baseFireRate = 0.5f;  // Base time between shots (in seconds)
    private float nextFireTime = 0f;   // Tracks the next time the weapon can fire

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        currentAmmo = maxAmmo; // Initialize with full ammo
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
                Debug.Log("Out of ammo! RIGHT CLICK to reload.");
            }
            return;
        }

        // Get fire rate and damage multipliers from PlayerStats
        float fireRateMultiplier = PlayerStats.Instance.fireRateMultiplier;
        float damageMultiplier = PlayerStats.Instance.damageMultiplier;

        // Calculate the time for the next shot using fire rate multiplier
        nextFireTime = Time.time + (baseFireRate / fireRateMultiplier);

        // Instantiating a bullet prefab
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

        // Playing audio whenever it fires
        if (audio != null)
        {
            audio.Play();
        }

        // Decrementing ammo after bullet instantiation
        currentAmmo--;
        Debug.Log($"Fired! Ammo left: {currentAmmo}");
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        // Optional: Add reload sound or animation here

        yield return new WaitForSeconds(reloadTime); // Wait for reload time

        currentAmmo = maxAmmo; // Refill ammo
        Debug.Log("Reloaded!");

        isReloading = false;
    }
}

