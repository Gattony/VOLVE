using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelector : MonoBehaviour
{
    public GameObject[] characterPreviews;
    public GameObject[] characterPlayablePrefabs;

    [Header("Character Info")]
    public string[] characterNames;
    [TextArea(2, 5)]
    public string[] characterDescriptions;

    [Header("UI References")]
    public Transform spawnPoint;
    public Button leftArrow;
    public Button rightArrow;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public CanvasGroup descriptionGroup;

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
        currentPreview.transform.localScale = Vector3.one * 0.5f;
        currentPreview.transform.localPosition = Vector3.zero;

        FloatIdle floatScript = currentPreview.GetComponent<FloatIdle>();
        if (floatScript != null) floatScript.enabled = false;

        Rigidbody2D rb = currentPreview.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        StartCoroutine(BounceIn(currentPreview, floatScript));

        // Update Name and Description UI
        StartCoroutine(UpdateCharacterInfo());
    }

    IEnumerator UpdateCharacterInfo()
    {
        // Fade out first
        yield return StartCoroutine(FadeDescription(0f, 0.15f));

        // Update the text
        nameText.text = characterNames[currentIndex];
        descriptionText.text = characterDescriptions[currentIndex];

        // Fade in
        yield return StartCoroutine(FadeDescription(1f, 0.15f));
    }

    IEnumerator FadeDescription(float targetAlpha, float duration)
    {
        float startAlpha = descriptionGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            descriptionGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        descriptionGroup.alpha = targetAlpha;
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

        while (elapsed < duration)
        {
            if (obj == null) yield break;
            obj.transform.localScale = Vector3.Lerp(startScale, overshoot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = overshoot;
        elapsed = 0f;

        while (elapsed < duration)
        {
            if (obj == null) yield break;
            obj.transform.localScale = Vector3.Lerp(overshoot, undershoot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = undershoot;
        elapsed = 0f;

        while (elapsed < duration / 2f)
        {
            if (obj == null) yield break;
            obj.transform.localScale = Vector3.Lerp(undershoot, finalScale, elapsed / (duration / 2f));
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = finalScale;
        obj.transform.localPosition = Vector3.zero;

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
