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
        if (player == null) return; // Prevents crash if player is missing

        Vector3 offset = Vector3.zero;

        if (actionJoystick != null)
        {
            Vector2 joystickDirection = actionJoystick.joystickDirec;
            offset = new Vector3(joystickDirection.x, joystickDirection.y, 0f) * maxOffset;
        }

        Camera mainCamera = Camera.main;

        targetPosition = player.position + offset;
        targetPosition.z = transform.position.z; // Keep original Z position

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
