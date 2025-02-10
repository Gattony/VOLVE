using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private float weaponDistance = 1.5f; // Distance of the weapon from the player

    private Rigidbody2D rb;
    public Animator animator;
    public Weapon weapon;

    private Vector2 movement;
    private Vector2 mousePosition;

    private const string horizontal = "Horizontal";
    private const string vertical = "Vertical";

    [Header("Joystick References")]
    public JoystickMovement movementJoystick; // Movement joystick
    public JoystickMovement actionJoystick; // Weapon rotation joystick

    private void Awake()
    {
        // Getting components in inspector
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Get joystick input for movement
        Vector2 joystickInput = movementJoystick.joystickDirec;

        // Check if movement joystick is being used (non-zero input), otherwise fallback to keyboard
        if (joystickInput != Vector2.zero)
        {
            movement = joystickInput; // Use joystick input for movement
        }
        else
        {
            // Fallback to keyboard input
            movement.Set(Input.GetAxisRaw(horizontal), Input.GetAxisRaw(vertical));
            movement.Normalize(); // Ensure movement vector is normalized
        }

        // Get mouse position in world space
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RotateWeaponAroundPlayer();
    }

    private void FixedUpdate()
    {
        // Applying movement with upgrades
        float speedMultiplier = PlayerStats.Instance.speedMultiplier;

        rb.velocity = movement * moveSpeed * speedMultiplier;
    }

    private void RotateWeaponAroundPlayer()
    {
        Vector2 aimDirection;

        // Check if action joystick is being used
        if (actionJoystick.joystickDirec != Vector2.zero)
        {
            // Use joystick direction for aiming
            aimDirection = actionJoystick.joystickDirec;
        }
        else
        {
            // Fallback to mouse aiming
            aimDirection = mousePosition - rb.position;
        }

        // Calculate the aim angle
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // Calculate the new position of the weapon
        Vector2 weaponPosition = rb.position + aimDirection.normalized * weaponDistance;
        weaponTransform.position = weaponPosition;

        // Rotate the weapon to face the aim direction
        weaponTransform.rotation = Quaternion.Euler(0, 0, aimAngle - 90);
    }
}
