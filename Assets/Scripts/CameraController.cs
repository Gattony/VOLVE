using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float displacementMultipiler = 0.15f;
    private float zPosition = -1;

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 cameraDisplacement = (mousePosition - playerTransform.position) * displacementMultipiler;

        Vector3 finalCameraPosition = playerTransform.position + cameraDisplacement;
        finalCameraPosition.z = zPosition;
        transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, zPosition);
        transform.position = finalCameraPosition;
    }
}
