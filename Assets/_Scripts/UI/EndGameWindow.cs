using Michsky.MUIP;
using UnityEngine;

// Handles the enabling and displaying of the results menu window along with
// any additional behaviors that need to occur once the game has finished
//
// This procedure can't be done with UnityEvents in the editor since by default the window is disabled 
public class EndGameWindow : MonoBehaviour
{
    [SerializeField]
    private ModalWindowManager _window;

    // Subscribe to Event Manager events on enable
    private void OnEnable()
    {
        EventManager.Game.OnGameEnd += EndGameActions;
    }

    // Unsubscribe to Event Manager events on disable
    private void OnDisable()
    {
        EventManager.Game.OnGameEnd -= EndGameActions;
    }

    // Event Handlers
    private void EndGameActions()
    {
        _window.Open();

        // Update the UI Creature Count Display inside of the results window,
        // since it has been disabled and would still be in default state
        EventManager.UI.OnAgentCountHeard?.Invoke();
    }
}
