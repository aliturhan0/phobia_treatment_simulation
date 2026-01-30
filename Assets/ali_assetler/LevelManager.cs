using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Added for Scene Loading

public class LevelManager : MonoBehaviour
{
    [Header("Assign Parts")]
    public GameObject level1Canvas; // Eski VictoryCanvas (Seviye 1 Bitiş)
    public GameObject level2Canvas; // Yeni Seviye 2 Bitiş Paneli
    public GameObject xrOrigin;      // The Player (to teleport)

    [Header("Level Settings")]
    public Transform level1SpawnPoint; // Level 1 Start
    public Transform level2SpawnPoint; // Level 2 Start (The Sky Platform)
    public AudioSource audioSource;
    public AudioClip victorySound;

    void Start()
    {
        // Hide victory screens at start
        if (level1Canvas) level1Canvas.SetActive(false);
        if (level2Canvas) level2Canvas.SetActive(false);
        
        // Auto-find XR Origin if missing (Common Helper)
        if (xrOrigin == null) xrOrigin = GameObject.Find("XR Origin");
    }

    // Call this when Player enters the End Zone (Level 1)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowVictory();
        }
    }

    public void ShowVictory()
    {
        Debug.Log("LEVEL 1 FINISHED! Showing Victory Screen.");
        if (level1Canvas) level1Canvas.SetActive(true);
        if (audioSource && victorySound) audioSource.PlayOneShot(victorySound);
    }
    
    public void ShowVictoryLevel2()
    {
        Debug.Log("LEVEL 2 FINISHED! Showing Victory Screen.");
        if (level2Canvas) level2Canvas.SetActive(true);
        if (audioSource && victorySound) audioSource.PlayOneShot(victorySound);
    }

    // BUTTON 1: NEXT LEVEL (From Level 1)
    public void GoToNextLevel()
    {
        TeleportTo(level2SpawnPoint);
        if (level1Canvas) level1Canvas.SetActive(false);
    }

    // BUTTON 2: REPLAY LEVEL 1
    public void ReplayLevel()
    {
        TeleportTo(level1SpawnPoint);
        if (level1Canvas) level1Canvas.SetActive(false);
    }

    // BUTTON 3: REPLAY LEVEL 2
    public void ReplayLevel2()
    {
        TeleportTo(level2SpawnPoint);
        if (level2Canvas) level2Canvas.SetActive(false);
    }

    // BUTTON 4: RETURN TO LEVEL 1 (From Level 2)
    public void ReturnToLevel1()
    {
        TeleportTo(level1SpawnPoint);
        if (level2Canvas) level2Canvas.SetActive(false);
    }

    // BUTTON 5: MAIN MENU (Loads Scene 0)
    public void LoadMainMenu()
    {
        Debug.Log("Loading Main Menu...");
        SceneManager.LoadScene(0);
    }

    private void TeleportTo(Transform target)
    {
        if (xrOrigin && target)
        {
            Debug.Log("Teleporting to: " + target.name);
            
            CharacterController cc = xrOrigin.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;

            xrOrigin.transform.position = target.position;
            xrOrigin.transform.rotation = target.rotation;

            if (cc) cc.enabled = true;

            // Robust Fix: Force close both panels whenever we teleport
            // This handles cases where the user might have assigned the wrong button event
            if (level1Canvas) level1Canvas.SetActive(false);
            if (level2Canvas) level2Canvas.SetActive(false);
        }
        else
        {
            Debug.LogError("Missing XR Origin or Target Spawn Point!");
        }
    }
}