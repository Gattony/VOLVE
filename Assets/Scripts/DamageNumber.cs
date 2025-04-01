using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float lifetime = 1f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += new Vector3(0, floatSpeed * Time.deltaTime, 0); // Move up
    }
}