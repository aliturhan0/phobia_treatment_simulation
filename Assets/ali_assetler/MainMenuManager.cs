using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Dropdown Containers")]
    public GameObject heightOptionsContainer; // The GameObject holding Nature & City buttons

    [Header("Scene Names")]
    public string natureSceneName = "ali";
    public string citySceneName = "onuralp";
    public string spiderSceneName = "ozcan2";

    void Start()
    {
        // Start fresh: Hide dropdown
        if (heightOptionsContainer) heightOptionsContainer.SetActive(false);

        // LOCK PLAYER MOVEMENT (STATIONARY MENU)
        DisablePlayerMovement();
    }

    void DisablePlayerMovement()
    {
        // Try to find standard XR Locomotion scripts and disable them
        var moveProviders = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.ActionBasedContinuousMoveProvider>();
        foreach (var move in moveProviders) move.enabled = false;

        var turnProviders = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.ActionBasedContinuousTurnProvider>();
        foreach (var turn in turnProviders) turn.enabled = false;
        
        var snapTurnProviders = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.ActionBasedSnapTurnProvider>();
        foreach (var snap in snapTurnProviders) snap.enabled = false;

        // Optional: Disable Locomotion System entirely if you want to be super strict
        // var locSystems = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.LocomotionSystem>();
        // foreach (var loc in locSystems) loc.enabled = false;

        Debug.Log("Main Menu: Player movement locked.");
    }

    // --- INTERACTION LOGIC ---

    public void ToggleHeightOptions()
    {
        Debug.Log("ToggleHeightOptions button clicked!"); // Log click

        if (heightOptionsContainer)
        {
            // Toggle visibility: If open -> close, If closed -> open
            bool isActive = heightOptionsContainer.activeSelf;
            heightOptionsContainer.SetActive(!isActive);
            Debug.Log("Container Toggled. New State: " + (!isActive));
        }
        else
        {
            Debug.LogError("HATA: 'Height Options Container' kutusu boş! Script'e objeyi sürüklememişsin.");
        }
    }

    // --- SCENE LOADING LOGIC ---

    public void LoadNatureScene()
    {
        Debug.Log("Loading Nature Scene: " + natureSceneName);
        SceneManager.LoadScene(natureSceneName);
    }

    public void LoadCityScene()
    {
        Debug.Log("Loading City Scene: " + citySceneName);
        SceneManager.LoadScene(citySceneName);
    }

    public void LoadSpiderScene()
    {
        Debug.Log("Loading Spider Scene: " + spiderSceneName);
        SceneManager.LoadScene(spiderSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
