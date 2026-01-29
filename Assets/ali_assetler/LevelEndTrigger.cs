using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    public enum Level
    {
        Level1,
        Level2
    }

    [Tooltip("Which level does this trigger finish?")]
    public Level levelToFinish;

    [Tooltip("Drag the LevelManager object here")]
    public LevelManager levelManager;

    private void OnTriggerEnter(Collider other)
    {
        // Detect Player via Tag and ensure LevelManager is assigned
        if (other.CompareTag("Player"))
        {
            if (levelManager == null)
            {
                Debug.LogError("LevelEndTrigger: LevelManager is not assigned!");
                return;
            }

            if (levelToFinish == Level.Level1)
            {
                levelManager.ShowVictory();
            }
            else if (levelToFinish == Level.Level2)
            {
                levelManager.ShowVictoryLevel2();
            }
        }
    }
}
