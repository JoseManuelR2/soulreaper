using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SpellMenu : MonoBehaviour
{
    public GameObject[] firePrefabs;
    public float radius = 0.2f;
    public float fireScale = 0.25f;
    public InputActionProperty gripAction;

    [Header("Audio")]
    public AudioClip menuOpenSound;

    public static List<GameObject> ActiveFires = new List<GameObject>();

    void Update()
    {
        if (gripAction.action == null) return;

        if (gripAction.action.WasPressedThisFrame()) SpawnPentagon();
        if (gripAction.action.WasReleasedThisFrame()) DestroyPentagon();
    }

    void SpawnPentagon()
    {
        if (WandController.Instance != null)
        {
            if (WandController.Instance.audioSource != null)
            {
                if (menuOpenSound != null)
                {
                    WandController.Instance.audioSource.pitch = 1f;
                    WandController.Instance.audioSource.time = 0.5f;
                    WandController.Instance.audioSource.PlayOneShot(menuOpenSound);
                }
                else
                {
                    Debug.LogWarning("SpellMenu: No has asignado el AudioClip 'menuOpenSound' en el Inspector.");
                }
            }
            else
            {
                Debug.LogError("SpellMenu: El WandController no tiene un AudioSource asignado o encontrado.");
            }
        WandController.Instance.SendHaptic(0.3f, 0.15f);
        }
        else
        {
            Debug.LogError("SpellMenu: No se puede reproducir sonido porque WandController.Instance es NULO.");
        }

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
        WandController.ProcessSpell();
        foreach (GameObject fire in new List<GameObject>(ActiveFires))
        {
            if (fire != null) Destroy(fire);
        }
        ActiveFires.Clear();
    }
}