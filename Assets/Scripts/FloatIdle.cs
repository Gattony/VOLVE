using UnityEngine;

public class FloatIdle : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float floatHeight = 10f;
    public float delay = 2f;

    private Vector3 startPos;
    private bool canFloat = false;

    void Start()
    {
        startPos = transform.localPosition;
        StartCoroutine(StartFloatingAfterDelay());
    }

    System.Collections.IEnumerator StartFloatingAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        canFloat = true;
    }

    void Update()
    {
        if (!canFloat) return;

        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = startPos + new Vector3(0f, yOffset, 0f);
    }
}
