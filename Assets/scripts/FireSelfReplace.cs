using UnityEngine;

public class FireSelfReplace : MonoBehaviour
{
    [Header("Prefab que reemplazará a esta llama")]
    public GameObject selectedFirePrefab;

    [Header("Escala del nuevo fuego")]
    public float replacementScale = 0.25f;

    private void Awake()
    {
        Debug.Log($"{name}: FireSelfReplace Awake called");

        // Comprobamos si hay Collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning($"{name}: No Collider found on this GameObject!");
        }
        else
        {
            Debug.Log($"{name}: Collider found, isTrigger = {col.isTrigger}");
        }

        // Comprobamos si hay Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning($"{name}: No Rigidbody found! Trigger may not fire with wand");
        }
        else
        {
            Debug.Log($"{name}: Rigidbody found, isKinematic = {rb.isKinematic}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{name}: OnTriggerEnter called with {other.name}, tag={other.tag}");

        // Filtramos solo el objeto que hará el reemplazo (por ejemplo, la wand)
        if (other.CompareTag("wand"))
        {
            Debug.Log($"{name}: Triggered by wand, replacing fire prefab");

            Vector3 spawnPos = transform.position;

            // Destruye esta bola de fuego
            Destroy(gameObject);
            Debug.Log($"{name}: Fire destroyed");

            // Instancia la bola de fuego seleccionada en su lugar
            if (selectedFirePrefab != null)
            {
                GameObject newFire = Instantiate(selectedFirePrefab, spawnPos, Quaternion.identity);
                newFire.transform.localScale = Vector3.one * replacementScale;
                Debug.Log($"{name}: Selected fire prefab instantiated at {spawnPos}");
            }
            else
            {
                Debug.LogWarning($"{name}: selectedFirePrefab is null!");
            }
        }
    }
}