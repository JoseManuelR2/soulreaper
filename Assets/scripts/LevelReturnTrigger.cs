using UnityEngine;

public class LevelReturnTrigger : MonoBehaviour
{
    public LevelSelector levelSelector;
    public DoorController levelDoor;

    private void Start()
    {
        if (levelSelector == null) levelSelector = Object.FindFirstObjectByType<LevelSelector>();
        if (levelDoor == null)
        {
            GameObject doorObj = GameObject.Find("Door_02_reinforced");
            if (doorObj != null) levelDoor = doorObj.GetComponent<DoorController>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || (other.transform.parent != null && other.transform.parent.CompareTag("Player")))
        {
            if (levelSelector != null && !string.IsNullOrEmpty(levelSelector.GetCurrentSceneName()))
            {
                GetComponent<Collider>().enabled = false;

                if (levelDoor != null) levelDoor.CloseDoor();
                levelSelector.UnloadCurrentLevel();
            }
        }
    }
}