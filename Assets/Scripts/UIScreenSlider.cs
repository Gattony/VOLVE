using UnityEngine;

public class UIScreenSlider : MonoBehaviour
{
    public RectTransform screenContainer;
    public float slideSpeed = 800f;

    private Vector2 targetPosition;

    private void Start()
    {
        // Start at the title screen (position 0)
        targetPosition = Vector2.zero;
    }

    private void Update()
    {
        screenContainer.anchoredPosition = Vector2.Lerp(
            screenContainer.anchoredPosition,
            targetPosition,
            Time.deltaTime * 5f
        );
    }

    public void SlideToCharacterSelect()
    {
        targetPosition = new Vector2(-1920f, 0f); // Adjust to match Canvas width
    }

    public void SlideToTitleScreen()
    {
        targetPosition = Vector2.zero;
    }
}
