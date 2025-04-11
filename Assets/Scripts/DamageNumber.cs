using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float lifetime = 1f;
    public float fadeSpeed = 1f;

    private TextMeshPro textComponent;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        textComponent = GetComponentInChildren<TextMeshPro>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize(float damage)
    {
        if (textComponent == null) return;
        textComponent.text = damage.ToString("F1"); // Show one decimal

    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += new Vector3(0, floatSpeed * Time.deltaTime, 0);

        if (canvasGroup != null)
        {
            canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
        }
    }
}
