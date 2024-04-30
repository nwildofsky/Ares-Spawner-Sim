using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// This script will initialize the controls set up in the new input system action mappings
// Also handles generic input reading from the setup action mappings
public class InputManager : MonoBehaviour
{
    // C# generated class from input actions asset
    private InputControls _controlScript;

    // Cached action mappings
    // Public static for other scripts as well
    public static InputAction TouchPressAction;
    public static InputAction TouchPositionAction;

    private void Awake()
    {
        _controlScript = new InputControls();

        TouchPressAction = _controlScript.Touch.Press;
        TouchPositionAction = _controlScript.Touch.Position;
    }

    // When the input manager is activated, all of the input actions are activated
    private void OnEnable()
    {
        _controlScript.Enable();

        TouchPressAction.performed += ApplyTouch;
    }

    // When the input manager is deactivated, all of the input actions are deactivated
    private void OnDisable()
    {
        _controlScript.Disable();

        TouchPressAction.performed -= ApplyTouch;
    }

    // Log the input values being read if there is even an issue with user input
    private void DebugTouch(InputAction.CallbackContext context)
    {
        float isPressed = TouchPressAction.ReadValue<float>();
        Vector2 position = TouchPositionAction.ReadValue<Vector2>();
        Debug.Log(isPressed + $" At X: {position.x} Y: {position.y}");
    }

    // Main method for handling non-ui input for tapping on the screen
    private void ApplyTouch(InputAction.CallbackContext context)
    {
        Vector2 touchPos = TouchPositionAction.ReadValue<Vector2>();

        // Check first if click hits a piece of UI
        // Not able to hit touchables that are under UI elements
        if (DidPressHitUI(touchPos))
            return;

        // If a ITouchable object is hit from the raycasted input, call its method
        if (DidPressHitTouchable(touchPos, out ITouchable t))
        {
            t.HandleTouch();
        }
    }

    // Perform a raycast from the screen position that was touched down into the 3D game world,
    // Searching for an ITouchable component that is on the Touchable layer
    //
    // Returns - true | false if a touchable component was found, and a reference to the component if it was
    private bool DidPressHitTouchable(Vector2 touchPos, out ITouchable touchedObj)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        LayerMask mask = LayerMask.GetMask("Touchable");
        // Raycasting a distance of 100 is enough to capture everything touchable inside the scene
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

    // Utilize the main EventSystem to check if touch input hits any UI elements with a raycast
    private bool DidPressHitUI(Vector2 touchPos)
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = new Vector2(touchPos.x, touchPos.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);
        return results.Count > 0;
    }
}
