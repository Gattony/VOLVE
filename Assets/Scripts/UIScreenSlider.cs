using UnityEngine;

public class UIScreenSlider : MonoBehaviour
{
    public RectTransform screenContainer;
    public float slideSpeed = 800f;

    [Header("Cinematic Bars")]
    public RectTransform topBar;
    public RectTransform bottomBar;
    public float barSlideDistance = 200f;
    public float barSlideSpeed = 5f;

    private Vector2 targetScreenPosition;
    private Vector2 topBarStartPosition;
    private Vector2 bottomBarStartPosition;
    private Vector2 topBarTarget;
    private Vector2 bottomBarTarget;

    private void Start()
    {
        targetScreenPosition = Vector2.zero;

        if (topBar != null)
        {
            topBarStartPosition = topBar.anchoredPosition;
            topBarTarget = topBarStartPosition; // Start at rest
        }

        if (bottomBar != null)
        {
            bottomBarStartPosition = bottomBar.anchoredPosition;
            bottomBarTarget = bottomBarStartPosition; // Start at rest
        }
    }


    private void Update()
    {
        // Move the screen
        screenContainer.anchoredPosition = Vector2.Lerp(
            screenContainer.anchoredPosition,
            targetScreenPosition,
            Time.deltaTime * 5f
        );

        // Move the top and bottom bars
        if (topBar != null)
            topBar.anchoredPosition = Vector2.Lerp(topBar.anchoredPosition, topBarTarget, Time.deltaTime * barSlideSpeed);

        if (bottomBar != null)
            bottomBar.anchoredPosition = Vector2.Lerp(bottomBar.anchoredPosition, bottomBarTarget, Time.deltaTime * barSlideSpeed);
    }

    public void SlideToCharacterSelect()
    {
        targetScreenPosition = new Vector2(-1920f, 0f);

        if (topBar != null)
            topBarTarget = topBarStartPosition + new Vector2(0f, -barSlideDistance); // TOP bar moves DOWN

        if (bottomBar != null)
            bottomBarTarget = bottomBarStartPosition + new Vector2(0f, +barSlideDistance); // BOTTOM bar moves UP
    }
    public void SlideToTitleScreen()
    {
        targetScreenPosition = Vector2.zero;

        if (topBar != null)
            topBarTarget = topBarStartPosition; // Return to original

        if (bottomBar != null)
            bottomBarTarget = bottomBarStartPosition; // Return to original
    }
}
