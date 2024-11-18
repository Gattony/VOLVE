using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Base Stats")]
    public float baseDamage = 10f;
    public float fireRate = 1f; 
    public float movementSpeed = 5f;

    [Header("Modifiers")]
    public float damageMultiplier = 1f;
    public float fireRateMultiplier = 1f;
    public float speedMultiplier = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Getters for final stats
    public float GetDamage() => baseDamage * damageMultiplier;
    public float GetFireRate() => fireRate * fireRateMultiplier;
    public float GetMovementSpeed() => movementSpeed * speedMultiplier;

}
