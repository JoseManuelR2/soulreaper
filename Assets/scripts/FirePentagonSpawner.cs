using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class FirePentagonSpawner : MonoBehaviour
{
    // Ahora podemos asignar 5 prefabs distintos desde el inspector
    public GameObject[] firePrefabs;
    public float radius = 0.2f;
    public float fireScale = 0.25f;

    public InputActionProperty gripAction;

    private List<GameObject> spawnedFires = new List<GameObject>();

    void Update()
    {
        if (gripAction.action == null)
        {
            Debug.LogWarning("Grip Action NO está asignada");
            return;
        }

        if (gripAction.action.WasPressedThisFrame())
        {
            SpawnPentagon();
        }

        if (gripAction.action.WasReleasedThisFrame())
        {
            DestroyPentagon();
        }
    }

    void SpawnPentagon()
    {
        if (firePrefabs == null || firePrefabs.Length < 5)
        {
            Debug.LogWarning("Debes asignar 5 prefabs distintos en firePrefabs");
            return;
        }

        Debug.Log("Spawning fire pentagon");

        int sides = 5;

        for (int i = 0; i < sides; i++)
        {
            float angle = i * Mathf.PI * 2 / sides;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            Vector3 localOffset = new Vector3(x, 0, z);
            Vector3 spawnPosition = transform.position + transform.TransformDirection(localOffset);

            // Instancia un prefab diferente para cada lado
            GameObject fire = Instantiate(firePrefabs[i], spawnPosition, Quaternion.identity);

            fire.transform.localScale = Vector3.one * fireScale;

            spawnedFires.Add(fire);

            Debug.Log("Spawned fire " + i);
        }
    }

    void DestroyPentagon()
    {
        Debug.Log("Destroying fire pentagon");

        foreach (GameObject fire in spawnedFires)
        {
            Destroy(fire);
        }

        spawnedFires.Clear();
    }
}