using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickMovement : MonoBehaviour
{
    public GameObject joystick;
    public GameObject joystickBG;
    public Vector2 joystickDirec;

    private Vector2 joystickTouchPos;
    private Vector2 joystickOriginalPos;
    private Vector2 joystickBGOriginalPos;
    private float joystickRadius;


    // Start is called before the first frame update
    void Start()
    {
        joystickOriginalPos = joystick.transform.localPosition;
        joystickBGOriginalPos = joystickBG.transform.position;
        joystickRadius = joystickBG.GetComponent<RectTransform>().sizeDelta.y / 4;
    }
    
    public void PointerDown()
    {
        joystickBG.transform.position = Input.mousePosition;
        joystickTouchPos = Input.mousePosition;
    }

    public void Drag(BaseEventData baseEventData)
    {
        PointerEventData pointerEventData = (PointerEventData)baseEventData;
        Vector2 dragPos = pointerEventData.position;

        joystickDirec = (dragPos - joystickTouchPos).normalized;

        float joystickDist = Mathf.Min(Vector2.Distance(dragPos,joystickTouchPos), joystickRadius);

        joystick.transform.localPosition = joystickDirec * joystickDist;
    }

    public void PointerUp()
    {
        joystickDirec = Vector2.zero;
        joystick.transform.localPosition = joystickOriginalPos;
        joystickBG.transform.position = joystickBGOriginalPos;
    }
}
