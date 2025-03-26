using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private float weaponDistance = 1.5f;
    [SerializeField] private float firingSpeedMultiplier = 0.6f;

    private Rigidbody2D rb;
    public Animator animator;
    public Weapon weapon;

    private Vector2 movement;
    private Vector2 lastMoveDirection = Vector2.right; // Stores last movement direction for tentacles

    private const string horizontal = "Horizontal";
    private const string vertical = "Vertical";

    [Header("Joystick References")]
    public JoystickMovement movementJoystick;
    public JoystickMovement actionJoystick;

    private bool isFiring = false;

    [Header("Tentacle Effect Settings")]
    public int tentacleCount = 5;
    public int segmentCount = 10;
    public float tentacleLength = 2f;
    public float waveSpeed = 1f;
    public float waveAmplitude = 0.15f;
    public float tentacleFollowSpeed = 5f;
    public Material tentacleMaterial;

    private List<LineRenderer> tentacles = new List<LineRenderer>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Initialize tentacles
        for (int i = 0; i < tentacleCount; i++)
        {
            GameObject tentacleObj = new GameObject("Tentacle_" + i);
            tentacleObj.transform.parent = transform;

            LineRenderer lineRenderer = tentacleObj.AddComponent<LineRenderer>();
            lineRenderer.positionCount = segmentCount;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = tentacleMaterial;
            tentacles.Add(lineRenderer);
        }
    }

    private void Update()
    {
        // Get joystick input for movement
        Vector2 joystickInput = movementJoystick.joystickDirec;

        if (joystickInput != Vector2.zero)
        {
            movement = joystickInput;
        }
        else
        {
            movement.Set(Input.GetAxisRaw(horizontal), Input.GetAxisRaw(vertical));
            movement.Normalize();
        }

        if (movement.magnitude > 0.1f)
        {
            lastMoveDirection = Vector2.Lerp(lastMoveDirection, movement, Time.deltaTime * tentacleFollowSpeed);
        }

        RotateWeaponAroundPlayer();
        AnimateTentacles();
    }

    private void FixedUpdate()
    {
        float speedMultiplier = PlayerStats.Instance.speedMultiplier;

        if (isFiring)
        {
            speedMultiplier *= firingSpeedMultiplier;
        }

        rb.velocity = movement * moveSpeed * speedMultiplier;
    }

    private void RotateWeaponAroundPlayer()
    {
        Vector2 aimDirection;

        if (actionJoystick.joystickDirec != Vector2.zero)
        {
            aimDirection = actionJoystick.joystickDirec;
            isFiring = true;
        }
        else
        {
            isFiring = false;
            return;
        }

        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        Vector2 weaponPosition = rb.position + aimDirection.normalized * weaponDistance;
        weaponTransform.position = weaponPosition;
        weaponTransform.rotation = Quaternion.Euler(0, 0, aimAngle - 90);
    }

    private void AnimateTentacles()
    {
        for (int i = 0; i < tentacles.Count; i++)
        {
            AnimateTentacle(tentacles[i], i);
        }
    }

    private void AnimateTentacle(LineRenderer tentacle, int index)
    {
        Vector3 startPos = transform.position;
        Vector3 direction = lastMoveDirection + (Vector2)Random.insideUnitCircle * 0.5f;
        direction.Normalize();

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            float waveOffset = Mathf.Sin(Time.time * waveSpeed + t * 3f + index * 2f) * waveAmplitude;

            Vector3 segmentPos = startPos + direction * (tentacleLength * t);
            segmentPos += new Vector3(waveOffset, waveOffset, 0);

            tentacle.SetPosition(i, segmentPos);
        }
    }
}
