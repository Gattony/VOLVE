using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    public SpriteRenderer targetSize;

    private void Start()
    {
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = targetSize.bounds.size.x / screenRatio;

        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = targetSize.bounds.size.y / 2;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = targetSize.bounds.size.y / 2 * differenceInSize;
        }
    }

}
