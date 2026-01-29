using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Assign Parts")]
    public GameObject victoryCanvas; // The "Congrats" UI Panel
    public GameObject xrOrigin;      // The Player (to teleport)

    [Header("Level Settings")]
    public Transform level1SpawnPoint; // Level 1 Start
    public Transform level2SpawnPoint; // Level 2 Start (The Sky Platform)
    public AudioSource audioSource;
    public AudioClip victorySound;

    void Start()
    {
        // Hide victory screen at start
        if (victoryCanvas) victoryCanvas.SetActive(false);
        
        // Auto-find XR Origin if missing (Common Helper)
        if (xrOrigin == null) xrOrigin = GameObject.Find("XR Origin");
    }

    // Call this when Player enters the End Zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowVictory();
        }
    }

    public void ShowVictory()
    {
        Debug.Log("LEVEL FINISHED! Showing Victory Screen.");
        if (victoryCanvas) victoryCanvas.SetActive(true);
        if (audioSource && victorySound) audioSource.PlayOneShot(victorySound);
    }

    // BUTTON 1: NEXT LEVEL
    public void GoToNextLevel()
    {
        TeleportTo(level2SpawnPoint);
    }

    // BUTTON 2: REPLAY LEVEL 1
    public void ReplayLevel()
    {
        TeleportTo(level1SpawnPoint);
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

            // Hide UI after teleport
            if (victoryCanvas) victoryCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("Missing XR Origin or Target Spawn Point!");
        }
    }
}
