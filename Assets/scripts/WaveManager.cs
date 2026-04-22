using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Esta clase permite configurar cada oleada desde el Inspector
[System.Serializable]
public class WaveConfig
{   
    [Tooltip("Total de enemigos que saldrán en esta ronda")]
    public int totalEnemies = 10;
    
    [Tooltip("Máximo de enemigos que pueden estar vivos a la vez en esta ronda")]
    public int maxSimultaneousEnemies = 3;
}

public class WaveManager : MonoBehaviour
{
    [Header("Configuración General")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints; 

    [Header("Configuración de Oleadas")]
    [Tooltip("Añade aquí las oleadas que quieras. El tamaño de la lista define el total de rondas.")]
    public List<WaveConfig> waves; 

    [Header("Estado Actual (Solo lectura)")]
    [SerializeField] private int currentWaveIndex = 0;
    [SerializeField] private int enemiesSpawnedInWave = 0;
    [SerializeField] private int currentEnemiesAlive = 0;

    private int lastSpawnIndex = -1; // Para evitar repetir el mismo punto de spawn seguido

    void Start()
    {
        if (waves.Count > 0)
        {
            StartCoroutine(WaveRoutine());
        }
        else
        {
            Debug.LogError("No has configurado ninguna oleada en el WaveManager.");
        }
    }

    IEnumerator WaveRoutine()
    {
        // Recorremos la lista de oleadas
        for (currentWaveIndex = 0; currentWaveIndex < waves.Count; currentWaveIndex++)
        {
            WaveConfig currentWave = waves[currentWaveIndex];
            
            enemiesSpawnedInWave = 0;

            // Mientras no hayamos spawneado todos los de la oleada, o queden vivos
            while (enemiesSpawnedInWave < currentWave.totalEnemies || currentEnemiesAlive > 0)
            {
                // Si aún quedan por spawnear y no hemos superado el límite simultáneo de ESTA oleada específica
                if (enemiesSpawnedInWave < currentWave.totalEnemies && currentEnemiesAlive < currentWave.maxSimultaneousEnemies)
                {
                    SpawnEnemy();
                }
                
                yield return new WaitForSeconds(0.5f); // Pequeña pausa para no saturar el juego
            }
            yield return new WaitForSeconds(3f); // Tiempo de respiro entre oleadas
        }
        
        Debug.Log("Todas las oleadas terminadas.");
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0) return;

        // --- Mejora en el sorteo de Spawn Points ---
        int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);

        // Si hay más de un punto de spawn, evitamos que repita el mismo nodo que el anterior
        if (spawnPoints.Length > 1)
        {
            while (randomIndex == lastSpawnIndex)
            {
                randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
            }
        }
        
        lastSpawnIndex = randomIndex;
        Transform selectedPoint = spawnPoints[randomIndex];
        // -------------------------------------------
        
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