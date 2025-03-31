using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private float weaponDistance = 2.5f;
    [SerializeField] private float firingSpeedMultiplier = 0.6f;

    private Rigidbody2D rb;
    public Animator animator;
    public Weapon weapon;

    private Vector2 movement;
    private Vector2 lastMoveDirection = Vector2.right;

    private const string horizontal = "Horizontal";
    private const string vertical = "Vertical";

    [Header("Joystick References")]
    public JoystickMovement movementJoystick;
    public JoystickMovement actionJoystick;

    private bool isFiring = false;

    [Header("Tentacle Effect Settings")]
    public int tentacleCount = 8;
    public int segmentCount = 12;
    public float tentacleMaxLength = 2f;
    public float tentacleRetractSpeed = 3f;
    public float tentacleStretchSpeed = 4f;
    public float tentacleGrabDelayMin = 0.2f;
    public float tentacleGrabDelayMax = 0.6f;
    public float tentacleSpread = 100f;
    public float tentacleCurlFactor = 0.6f;
    public float tentacleWiggleIntensity = 0.3f;
    public Material tentacleMaterial;

    private List<LineRenderer> tentacles = new List<LineRenderer>();
    private List<Vector3> tentacleTargets = new List<Vector3>();
    private float[] tentacleTimers;
    private float[] tentacleWiggleOffsets;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        tentacleTimers = new float[tentacleCount];
        tentacleWiggleOffsets = new float[tentacleCount];

        for (int i = 0; i < tentacleCount; i++)
        {
            GameObject tentacleObj = new GameObject("Tentacle_" + i);
            tentacleObj.transform.parent = transform;

            LineRenderer lineRenderer = tentacleObj.AddComponent<LineRenderer>();
            lineRenderer.positionCount = segmentCount;
            lineRenderer.startWidth = 0.13f;
            lineRenderer.endWidth = 0.08f;
            lineRenderer.material = tentacleMaterial;
            tentacles.Add(lineRenderer);

            tentacleTargets.Add(transform.position);
            tentacleTimers[i] = Random.Range(tentacleGrabDelayMin, tentacleGrabDelayMax);
            tentacleWiggleOffsets[i] = Random.Range(0f, 2f * Mathf.PI);
        }
    }

    private void Update()
    {
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
            lastMoveDirection = Vector2.Lerp(lastMoveDirection, movement, Time.deltaTime * 10f);
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false); 
        }

        RotateWeaponAroundPlayer();
        UpdateTentacles();
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

    private Coroutine fadeCoroutine;

    private IEnumerator FadeWeapon(float targetAlpha)
    {
        SpriteRenderer weaponSprite = weaponTransform.GetComponent<SpriteRenderer>();
        if (weaponSprite == null) yield break; // Exit if no SpriteRenderer

        float startAlpha = weaponSprite.color.a;
        float duration = 0.15f; 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            weaponSprite.color = new Color(weaponSprite.color.r, weaponSprite.color.g, weaponSprite.color.b, newAlpha);
            yield return null;
        }

        weaponSprite.color = new Color(weaponSprite.color.r, weaponSprite.color.g, weaponSprite.color.b, targetAlpha); // Ensure final alpha is set
    }

    private void RotateWeaponAroundPlayer()
    {
        Vector2 aimDirection;
        SpriteRenderer weaponSprite = weaponTransform.GetComponent<SpriteRenderer>();

        if (actionJoystick.joystickDirec != Vector2.zero)
        {
            aimDirection = actionJoystick.joystickDirec;
            isFiring = true;

            if (weaponSprite.color.a < 1f)
            {
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeWeapon(1f)); // Fade in
            }
        }
        else
        {
            isFiring = false;

            if (weaponSprite.color.a > 0f) 
            {
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeWeapon(0f)); 
            }
            return;
        }

        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        Vector2 weaponPosition = rb.position + aimDirection.normalized * weaponDistance;
        weaponTransform.position = weaponPosition;
        weaponTransform.rotation = Quaternion.Euler(0, 0, aimAngle - 90);
    }

    private void UpdateTentacles()
    {
        for (int i = 0; i < tentacles.Count; i++)
        {
            tentacleTimers[i] -= Time.deltaTime;

            if (tentacleTimers[i] <= 0)
            {
                Vector3 grabPoint = transform.position +
                    (Quaternion.Euler(0, 0, (i - tentacleCount / 2f) * tentacleSpread + Random.Range(-10f, 10f)) * lastMoveDirection).normalized *
                    Random.Range(tentacleMaxLength * 0.6f, tentacleMaxLength);

                grabPoint += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

                tentacleTargets[i] = grabPoint;
                tentacleTimers[i] = Random.Range(tentacleGrabDelayMin, tentacleGrabDelayMax);
            }

            AnimateTentacle(tentacles[i], tentacleTargets[i], i);
        }
    }

    private void AnimateTentacle(LineRenderer tentacle, Vector3 target, int index)
    {
        Vector3 startPos = transform.position;
        Vector3 direction = (target - startPos).normalized;
        float length = Vector3.Distance(startPos, target);

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            float curlOffset = Mathf.Sin(t * Mathf.PI) * tentacleCurlFactor;
            float wiggleOffset = Mathf.Sin(Time.time * 3f + tentacleWiggleOffsets[index] + i * 0.5f) * tentacleWiggleIntensity * (1 - t);

            Vector3 segmentPos = startPos + direction * (length * t);
            segmentPos += new Vector3(curlOffset + wiggleOffset, -curlOffset + wiggleOffset, 0);

            tentacle.SetPosition(i, segmentPos);
        }
    }
}
