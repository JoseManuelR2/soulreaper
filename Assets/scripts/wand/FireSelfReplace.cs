using System;
using UnityEngine;

public class FireSelfReplace : MonoBehaviour
{
    
    public GameObject selectedFirePrefab;
    public String color;
    public float replacementScale = 0.25f;

    private void Awake()
    {
        // Registrar este fuego en el registro global
        FirePentagonSpawner.ActiveFires.Add(gameObject);

        // Aseguramos Rigidbody si no existe
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void OnDestroy()
    {
        // Eliminar del registro global cuando se destruye
        FirePentagonSpawner.ActiveFires.Remove(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("wand"))
        {
            Vector3 pos = transform.position;
            WandSwitcher.Spell.Add(color);
            Destroy(gameObject);

            if (selectedFirePrefab != null)
            {
                GameObject newFire = Instantiate(selectedFirePrefab, pos, Quaternion.identity);
                newFire.transform.localScale = Vector3.one * replacementScale;

                FirePentagonSpawner.ActiveFires.Add(newFire);
            }
        }
    }
}