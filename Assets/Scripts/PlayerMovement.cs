using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private Transform weaponTransform; // Reference to the weapon's transform
    [SerializeField] private float weaponDistance = 1.5f; // Distance of the weapon from the player

    private Rigidbody2D rb;
    public Animator animator;
    public Weapon weapon;

    private Vector2 movement;
    private Vector2 mousePosition;

    private const string horizontal = "Horizontal";
    private const string vertical = "Vertical";

    private void Awake()
    {
        //Getting componenets in inspector
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Handle movement input
        movement.Set(Input.GetAxisRaw(horizontal), Input.GetAxisRaw(vertical));


        if (Input.GetMouseButtonDown(0))
        {
            weapon.Fire();
        }

        // Get mouse position in world space
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Rotate weapon around player towards mouse position
        RotateWeaponAroundPlayer();
    }

    private void FixedUpdate()
    {
        // Apply movement
        rb.velocity = movement * moveSpeed;
    }

    private void RotateWeaponAroundPlayer()
    {
        // Calculate direction and angle to the mouse
        Vector2 aimDirection = mousePosition - rb.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // Calculate the new position of the weapon
        Vector2 weaponPosition = rb.position + aimDirection.normalized * weaponDistance;
        weaponTransform.position = weaponPosition;

        // Rotate the weapon to face the mouse
        weaponTransform.rotation = Quaternion.Euler(0, 0, aimAngle - 90);
    }
}