using UnityEngine;

public class WaveStartTrigger : MonoBehaviour
{
    public WaveManager waveManager;
    
    public DoorController levelDoor;

    public LevelSelector selector;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || (other.transform.parent != null && other.transform.parent.CompareTag("Player")))
        {
            GetComponent<Collider>().enabled = false;

            if (levelDoor != null) levelDoor.CloseDoor();

            if (selector != null && AudioManager.Instance != null)
            {
                string currentScene = selector.GetCurrentSceneName();
                if (currentScene == selector.level1Scene) AudioManager.Instance.PlayLoopMusic(AudioManager.Instance.level1Loop);
                else if (currentScene == selector.level2Scene) AudioManager.Instance.PlayLoopMusic(AudioManager.Instance.level2Loop);
                else if (currentScene == selector.level3Scene) AudioManager.Instance.PlayLoopMusic(AudioManager.Instance.level3Loop);
            }

            if (selector != null) selector.ResetInteractables();
            
            if (waveManager != null) waveManager.StartWaves();
            
            else Debug.LogError("WaveStartTrigger: No tengo WaveManager");

        }
    }
}