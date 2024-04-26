using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputControls controlScript;

    public static InputAction TouchPressAction;
    public static InputAction TouchPositionAction;

    private void Awake()
    {
        controlScript = new InputControls();

        TouchPressAction = controlScript.Touch.Press;
        TouchPositionAction = controlScript.Touch.Position;
    }

    private void OnEnable()
    {
        controlScript.Enable();

        //TouchPressAction.performed += DebugTouch;
        TouchPressAction.performed += CalculateTouch;
    }

    private void OnDisable()
    {
        controlScript.Disable();

        //TouchPressAction.performed -= DebugTouch;
        TouchPressAction.performed -= CalculateTouch;
    }

    private void DebugTouch(InputAction.CallbackContext context)
    {
        float isPressed = TouchPressAction.ReadValue<float>();
        Vector2 position = TouchPositionAction.ReadValue<Vector2>();
        Debug.Log(isPressed + $" At X: {position.x} Y: {position.y}");
    }

    private void CalculateTouch(InputAction.CallbackContext context)
    {
        Vector2 touchPos = TouchPositionAction.ReadValue<Vector2>();

        // Check first if click hits a piece of UI
        // Not able to hit touchables that are under UI elements
        if (DidPressHitUI(touchPos))
            return;

        if (DidPressHitTouchable(touchPos, out ITouchable t))
        {
            t.HandleTouch();
        }
    }

    private bool DidPressHitTouchable(Vector2 touchPos, out ITouchable touchedObj)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        LayerMask mask = LayerMask.GetMask("Touchable");
        if (Physics.Raycast(ray, out RaycastHit hit, 100, mask))
        {
            if (hit.collider.TryGetComponent<ITouchable>(out ITouchable touchable))
            {
                touchedObj = touchable;
                return true;
            }
        }

        touchedObj = null;
        return false;
    }

    private bool DidPressHitUI(Vector2 touchPos)
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = new Vector2(touchPos.x, touchPos.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);
        return results.Count > 0;
    }
}
