using System.Collections;
using UnityEngine;
using TMPro;

public class FlashingText : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public Color colorA = Color.white;
    public Color colorB = Color.green;
    public float flashInterval = 0.5f;

    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (textMeshPro == null)
            textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private void OnDisable()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
    }

    private IEnumerator FlashCoroutine()
    {
        bool isColorA = true;

        while (true)
        {
            textMeshPro.color = isColorA ? colorA : colorB;
            isColorA = !isColorA;

            yield return new WaitForSecondsRealtime(flashInterval);
        }
    }
}
