using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickMovement : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public GameObject joystick;
    public GameObject joystickBG;
    public Vector2 joystickDirec;

    private Vector2 joystickTouchPos;
    private Vector2 joystickOriginalPos;
    private Vector2 joystickBGOriginalPos;
    private float joystickRadius;

    private int activeTouchID = -1; // Track which finger is using this joystick

    void Start()
    {
        joystickOriginalPos = joystick.transform.localPosition;
        joystickBGOriginalPos = joystickBG.transform.position;
        joystickRadius = joystickBG.GetComponent<RectTransform>().sizeDelta.y / 4;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (activeTouchID == -1) // Only register if not already in use
        {
            activeTouchID = eventData.pointerId;
            joystickBG.transform.position = eventData.position;
            joystickTouchPos = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != activeTouchID) return; // Ignore other touches

        Vector2 dragPos = eventData.position;
        joystickDirec = (dragPos - joystickTouchPos).normalized;

        float joystickDist = Mathf.Min(Vector2.Distance(dragPos, joystickTouchPos), joystickRadius);
        joystick.transform.localPosition = joystickDirec * joystickDist;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId == activeTouchID) // Only reset if it's the same touch
        {
            activeTouchID = -1;
            joystickDirec = Vector2.zero;
            joystick.transform.localPosition = joystickOriginalPos;
            joystickBG.transform.position = joystickBGOriginalPos;
        }
    }
}
