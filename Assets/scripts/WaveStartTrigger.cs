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

            if (selector != null) selector.ResetInteractables();
            
            if (waveManager != null) waveManager.StartWaves();
            
            else Debug.LogError("WaveStartTrigger: No tengo WaveManager");

        }
    }
}