using UnityEngine;
using UnityEngine.SceneManagement;

// Simple monobehavior with public static methods linking directly to the SceneManager
// Useful for hooking up to UI elements with UnityEvents
public class SceneAPI : MonoBehaviour
{
    // Load the next scene with the build index number
    public static void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    // Load the next scene with the scene name number
    public static void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    // Load the currently active scene to restart from the beginning
    public static void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
