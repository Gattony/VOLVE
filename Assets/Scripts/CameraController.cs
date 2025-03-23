using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; // Assign in Inspector
    public float followSpeed = 2f;
    public float maxOffset = 3f;

    private Vector3 targetPosition;

    [Header("Joystick Reference")]
    public JoystickMovement actionJoystick;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("CameraController: Player reference is missing! Assign it in the Inspector.");
        }
    }

    private void Update()
    {
        if (player == null) return;

        Vector3 offset = Vector3.zero;

        if (actionJoystick != null)
        {
            Vector2 joystickDirection = actionJoystick.joystickDirec;
            offset = new Vector3(joystickDirection.x, joystickDirection.y, 0f) * maxOffset;
        }

        targetPosition = player.position + offset;
        targetPosition.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
    public void ShakeCameraOnce(float intensity = 0.15f, float duration = 0.08f)
    {
        StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector2 shakeOffset = Random.insideUnitCircle * intensity;
            transform.position = originalPos + new Vector3(shakeOffset.x, shakeOffset.y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }
}