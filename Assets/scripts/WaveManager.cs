using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WaveConfig
{   
    [Tooltip("Total de enemigos que saldrán en esta ronda")]
    public int totalEnemies = 10;
    
    [Tooltip("Máximo de enemigos que pueden estar vivos a la vez en esta ronda")]
    public int maxSimultaneousEnemies = 3;

    [Tooltip("Tiempo hasta la siguiente ronda")]
    public int breakTime = 5;
}

public class WaveManager : MonoBehaviour
{
    [Header("Configuración General")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints; 
    public DoorController levelDoor;

    [Header("Configuración de Oleadas")]
    [Tooltip("Añade aquí las oleadas que quieras. El tamaño de la lista define el total de rondas.")]
    public List<WaveConfig> waves; 

    [Header("Estado Actual (Solo lectura)")]
    [SerializeField] private int currentWaveIndex = 0;
    [SerializeField] private int enemiesSpawnedInWave = 0;
    [SerializeField] private int currentEnemiesAlive = 0;

    private VRVictoryUI vrVictoryUI;
    private int lastSpawnIndex = -1;

    void Start()
    {
        Camera cam = Camera.main;

        if (cam != null)
        {
            vrVictoryUI = cam.GetComponent<VRVictoryUI>();
        }

        if (levelDoor == null)
        {
            GameObject doorObj = GameObject.Find("Door_02_reinforced");
            if (doorObj != null) levelDoor = doorObj.GetComponent<DoorController>();
        }

        GameObject startTriggerObj = GameObject.Find("DoorTrigger_Start");
        if (startTriggerObj != null)
        {
            WaveStartTrigger trigger = startTriggerObj.GetComponent<WaveStartTrigger>();
            if (trigger != null)
            {
                trigger.waveManager = this; 
                trigger.levelDoor = this.levelDoor; 
                Debug.Log("WaveManager: Nivel cargado y enlazado al Lobby correctamente.");
            }
        }
        else
        {
            Debug.LogWarning("WaveManager: No encontré 'DoorTrigger_Start' en el Lobby.");
        }
    }

    public void StartWaves()
    {

        if (levelDoor == null)
        {
            GameObject doorObj = GameObject.Find("Door_02_reinforced");
            if (doorObj != null)
            {
                levelDoor = doorObj.GetComponent<DoorController>();
            }

        }

        if (waves.Count > 0)
        {
            StartCoroutine(WaveRoutine());
        }
        else
        {
            Debug.LogError("No has configurado ninguna oleada en el WaveManager.");
        }
}

    public void StopWaves()
    {
        StopAllCoroutines();
    }

    IEnumerator WaveRoutine()
    {
        for (currentWaveIndex = 0; currentWaveIndex < waves.Count; currentWaveIndex++)
        {
            WaveConfig currentWave = waves[currentWaveIndex];
            
            enemiesSpawnedInWave = 0;

            while (enemiesSpawnedInWave < currentWave.totalEnemies || currentEnemiesAlive > 0)
            {
                if (enemiesSpawnedInWave < currentWave.totalEnemies && currentEnemiesAlive < currentWave.maxSimultaneousEnemies)
                {
                    SpawnEnemy();
                }
                
                yield return new WaitForSeconds(0.5f);
            }
            
            if (currentWaveIndex < waves.Count - 1 && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.nextWaveSound);
            }

            yield return new WaitForSeconds(currentWave.breakTime); 
        }
        
        Debug.Log("Todas las oleadas terminadas.");

        LevelSelector selector = Object.FindFirstObjectByType<LevelSelector>();
        if (selector != null)
        {
            string currentScene = selector.GetCurrentSceneName();
            if (currentScene == selector.level1Scene) LevelSelector.level1Completed = true;
            else if (currentScene == selector.level2Scene) LevelSelector.level2Completed = true;
        }

        if (AudioManager.Instance != null)
        {

            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.victorySound);
            if (vrVictoryUI != null)
            {
                vrVictoryUI.ShowVictory();
            }
            float delay = AudioManager.Instance.victorySound != null ? AudioManager.Instance.victorySound.length : 2f;
            yield return new WaitForSeconds(delay);
            AudioManager.Instance.PlayNarrative(AudioManager.Instance.completeLevelNarrative);
        }

        if (levelDoor != null)
        {
            levelDoor.OpenDoor();
        }

        GameObject endTriggerObj = GameObject.Find("DoorTrigger_End");
        if (endTriggerObj != null)
        {
            Collider endCollider = endTriggerObj.GetComponent<Collider>();
            if (endCollider != null)
            {
                endCollider.enabled = true;
            }
        }
        else
        {
            Debug.LogWarning("WaveManager: No encontré el objeto 'DoorTrigger_End' para activarlo.");
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0) return;

        int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);

        if (spawnPoints.Length > 1)
        {
            while (randomIndex == lastSpawnIndex)
            {
                randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
            }
        }
        
        lastSpawnIndex = randomIndex;
        Transform selectedPoint = spawnPoints[randomIndex];
        
        GameObject newEnemy = Instantiate(enemyPrefab, selectedPoint.position, selectedPoint.rotation);
        
        currentEnemiesAlive++;
        enemiesSpawnedInWave++;

        EnemyController enemyScript = newEnemy.GetComponent<EnemyController>();
        if (enemyScript != null)
        {
            enemyScript.OnEnemyDeath += HandleEnemyDeath;
        }
        else
        {
            Debug.LogWarning("El prefab del enemigo no tiene el script EnemyController.");
        }
    }

    void HandleEnemyDeath()
    {
        currentEnemiesAlive--;
    }
}