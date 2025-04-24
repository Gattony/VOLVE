using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    public GameObject[] characterPreviews;            // Lightweight preview prefabs
    public GameObject[] characterPlayablePrefabs;     // Full versions used in gameplay

    public Transform spawnPoint;                      // Incubator display spot
    public Button leftArrow;
    public Button rightArrow;

    private int currentIndex = 0;
    private GameObject currentPreview;

    private void Start()
    {
        leftArrow.onClick.AddListener(SelectPreviousCharacter);
        rightArrow.onClick.AddListener(SelectNextCharacter);

        ShowCharacter(currentIndex);
    }

    void ShowCharacter(int index)
    {
        if (currentPreview != null)
            Destroy(currentPreview);

        currentPreview = Instantiate(characterPreviews[index], spawnPoint.position, Quaternion.identity, spawnPoint);
        currentPreview.transform.SetParent(spawnPoint, false);
        currentPreview.transform.localScale = Vector3.one * 0.5f; // Start small

        // Reset localPosition so floating always starts from zero
        currentPreview.transform.localPosition = Vector3.zero;

        // Disable FloatIdle during animation
        FloatIdle floatScript = currentPreview.GetComponent<FloatIdle>();
        if (floatScript != null) floatScript.enabled = false;

        // Disable physics
        Rigidbody2D rb = currentPreview.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        StartCoroutine(BounceIn(currentPreview, floatScript));
    }

    IEnumerator BounceIn(GameObject obj, FloatIdle floatScript)
    {
        if (obj == null) yield break;

        Vector3 startScale = obj.transform.localScale;
        Vector3 overshoot = Vector3.one * 1.05f;
        Vector3 undershoot = Vector3.one * 0.95f;
        Vector3 finalScale = Vector3.one;

        float duration = 0.15f;
        float elapsed = 0f;

        // Overshoot
        while (elapsed < duration)
        {
            if (obj == null) yield break;
            obj.transform.localScale = Vector3.Lerp(startScale, overshoot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = overshoot;
        elapsed = 0f;

        // Undershoot
        while (elapsed < duration)
        {
            if (obj == null) yield break;
            obj.transform.localScale = Vector3.Lerp(overshoot, undershoot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = undershoot;
        elapsed = 0f;

        // Settle
        while (elapsed < duration / 2f)
        {
            if (obj == null) yield break;
            obj.transform.localScale = Vector3.Lerp(undershoot, finalScale, elapsed / (duration / 2f));
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = finalScale;

        // Reset floating offset to exact zero before enabling
        obj.transform.localPosition = Vector3.zero;

        // Re-enable FloatIdle
        if (floatScript != null) floatScript.enabled = true;
    }


    void SelectNextCharacter()
    {
        currentIndex = (currentIndex + 1) % characterPreviews.Length;
        ShowCharacter(currentIndex);
    }

    void SelectPreviousCharacter()
    {
        currentIndex = (currentIndex - 1 + characterPreviews.Length) % characterPreviews.Length;
        ShowCharacter(currentIndex);
    }

    public void ConfirmSelection()
    {
        CharacterSelectionData.SelectedCharacterPrefab = characterPlayablePrefabs[currentIndex];
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame");
    }
}
