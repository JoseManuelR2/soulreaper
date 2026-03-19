using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class FirePentagonSpawner : MonoBehaviour
{
    public GameObject[] firePrefabs;
    public float radius = 0.2f;
    public float fireScale = 0.25f;
    public InputActionProperty gripAction;

    public static List<GameObject> ActiveFires = new List<GameObject>();

    void Update()
    {
        if (gripAction.action == null) return;

        if (gripAction.action.WasPressedThisFrame()) SpawnPentagon();
        if (gripAction.action.WasReleasedThisFrame()) DestroyPentagon();
    }

    void SpawnPentagon()
    {
        int sides = firePrefabs.Length;
        for (int i = 0; i < sides; i++)
        {
            float angle = i * Mathf.PI * 2 / sides;
            Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Vector3 pos = transform.position + transform.TransformDirection(offset);

            GameObject fire = Instantiate(firePrefabs[i], pos, Quaternion.identity);
            fire.transform.localScale = Vector3.one * fireScale;
        }
    }

    void DestroyPentagon()
    {
        WandSwitcher.ProcessSpell();
        foreach (GameObject fire in new List<GameObject>(ActiveFires))
        {
            if (fire != null) Destroy(fire);
        }
        ActiveFires.Clear();
    }
}