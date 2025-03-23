using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private float weaponDistance = 1.5f; // Distance of the weapon from the player
    [SerializeField] private float firingSpeedMultiplier = 0.5f; // Movement speed reduction while firing

    private Rigidbody2D rb;
    public Animator animator;
    public Weapon weapon;

    private Vector2 movement;

    private const string horizontal = "Horizontal";
    private const string vertical = "Vertical";

    [Header("Joystick References")]
    public JoystickMovement movementJoystick; // Movement joystick
    public JoystickMovement actionJoystick; // Weapon rotation joystick

    private bool isFiring = false; // Track if the player is currently firing

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Get joystick input for movement
        Vector2 joystickInput = movementJoystick.joystickDirec;

        // Use joystick movement if available, otherwise use keyboard
        if (joystickInput != Vector2.zero)
        {
            movement = joystickInput;
        }
        else
        {
            movement.Set(Input.GetAxisRaw(horizontal), Input.GetAxisRaw(vertical));
            movement.Normalize();
        }

        RotateWeaponAroundPlayer();
    }

    private void FixedUpdate()
    {
        float speedMultiplier = PlayerStats.Instance.speedMultiplier;

        // Reduce movement speed when firing
        if (isFiring)
        {
            speedMultiplier *= firingSpeedMultiplier;
        }

        rb.velocity = movement * moveSpeed * speedMultiplier;
    }

    private void RotateWeaponAroundPlayer()
    {
        Vector2 aimDirection;

        // Use joystick input if available
        if (actionJoystick.joystickDirec != Vector2.zero)
        {
            aimDirection = actionJoystick.joystickDirec;
            isFiring = true; // Player is actively aiming/firing
        }
        else
        {
            isFiring = false; // Stop firing if joystick is not used
            return;
        }

        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        Vector2 weaponPosition = rb.position + aimDirection.normalized * weaponDistance;
        weaponTransform.position = weaponPosition;
        weaponTransform.rotation = Quaternion.Euler(0, 0, aimAngle - 90);
    }
}
