using UnityEngine;
using TMPro;

public class FloatingDamage : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float fadeSpeed = 1f;

    private TextMeshProUGUI damageText;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        damageText = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize(float damage, Color textColor)
    {
        damageText.text = damage.ToString();
        damageText.color = textColor;
        Destroy(gameObject, 1f); // Destroy after 1 second
    }

    private void Update()
    {
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
        canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
    }
}
