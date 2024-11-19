using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Player and Mouse Settings")]
    [SerializeField] private Transform playerTransform;       
    [SerializeField] private float displacementMultipiler = 0.15f; // Controls how much the camera moves towards the mouse
    private float zPosition = -10f;                            

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.1f;       
    [SerializeField] private float shakeMagnitude = 0.1f;      
    private Vector3 shakeOffset = Vector3.zero;                // Offset applied during the shake
    private Coroutine shakeCoroutine;

    private void Update()
    {
        // Calculate mouse-based camera displacement
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 cameraDisplacement = (mousePosition - playerTransform.position) * displacementMultipiler;

        // Calculate final camera position, including shake
        Vector3 finalCameraPosition = playerTransform.position + cameraDisplacement + shakeOffset;
        finalCameraPosition.z = zPosition;

        // Set the camera position
        transform.position = finalCameraPosition;
    }

 
    public void TriggerShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine); // Stop any existing shake
        }
        shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            shakeOffset = new Vector3(offsetX, offsetY, 0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero; // Reset shake offset after shake
    }
}
