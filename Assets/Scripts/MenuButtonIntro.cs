using UnityEngine;

public class MenuButtonIntro : MonoBehaviour
{
    public CanvasGroup buttonGroup; 
    public float delay = 2f;        // time before intro ends
    public float fadeDuration = 1f; 

    private void Start()
    {
        buttonGroup.alpha = 0;
        buttonGroup.interactable = false;
        buttonGroup.blocksRaycasts = false;
        StartCoroutine(FadeIn());
    }

    private System.Collections.IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(delay);

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            buttonGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        buttonGroup.alpha = 1;
        buttonGroup.interactable = true;
        buttonGroup.blocksRaycasts = true;
    }
}
