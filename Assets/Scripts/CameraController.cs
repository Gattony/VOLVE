using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 2f;
    public float maxOffset = 3f;

    private Vector3 targetPosition;
    private bool isPanning = false;
    private bool disableJoystickOffset = false;

    private Coroutine continuousShakeCoroutine; 
    private Vector3 shakeOffset = Vector3.zero;  

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

        if (isPanning)
        {
            transform.position += shakeOffset;
            return;
        }

        Vector3 offset = Vector3.zero;

        if (!disableJoystickOffset && actionJoystick != null)
        {
            Vector2 joystickDirection = actionJoystick.joystickDirec;
            offset = new Vector3(joystickDirection.x, joystickDirection.y, 0f) * maxOffset;
        }

        targetPosition = player.position + offset;
        targetPosition.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, targetPosition + shakeOffset, followSpeed * Time.deltaTime);
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
            Vector2 shake = Random.insideUnitCircle * intensity;
            transform.position = originalPos + new Vector3(shake.x, shake.y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }

    public void StartContinuousShake(float intensity = 0.05f) 
    {
        if (continuousShakeCoroutine != null)
            StopCoroutine(continuousShakeCoroutine);

        continuousShakeCoroutine = StartCoroutine(ContinuousShakeCoroutine(intensity));
    }

    public void StopContinuousShake()
    {
        if (continuousShakeCoroutine != null)
        {
            StopCoroutine(continuousShakeCoroutine);
            continuousShakeCoroutine = null;
            shakeOffset = Vector3.zero; 
        }
    }

    private IEnumerator ContinuousShakeCoroutine(float intensity)
    {
        while (true)
        {
            Vector2 randomPoint = Random.insideUnitCircle * intensity;
            Vector3 targetShake = new Vector3(randomPoint.x, randomPoint.y, 0f);
            shakeOffset = Vector3.Lerp(shakeOffset, targetShake, 5f * Time.unscaledDeltaTime);
            yield return null;
        }
    }


    public void PanToPlayer(float duration = 2f)
    {
        StartCoroutine(PanToPlayerCoroutine(duration));
    }

    private IEnumerator PanToPlayerCoroutine(float duration)
    {
        isPanning = true;
        disableJoystickOffset = true;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(player.position.x, player.position.y, start.z);

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / duration;
            t = t * t * (3f - 2f * t);
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        transform.position = end;

        isPanning = false;

        yield return new WaitForSecondsRealtime(0.25f);
        disableJoystickOffset = false;
    }
}
