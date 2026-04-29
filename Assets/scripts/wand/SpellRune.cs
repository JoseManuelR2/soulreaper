using System;
using UnityEngine;

public class SpellRune : MonoBehaviour
{
    
    public GameObject selectedFirePrefab;
    public String color;
    public float replacementScale = 0.25f;

    private void Awake()
    {
        SpellMenu.ActiveFires.Add(gameObject);

        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void OnDestroy()
    {
        SpellMenu.ActiveFires.Remove(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("wand"))
        {
            Vector3 pos = transform.position;
            WandController.Instance.AddRune(color);
            Destroy(gameObject);

            if (selectedFirePrefab != null)
            {
                GameObject newFire = Instantiate(selectedFirePrefab, pos, Quaternion.identity);
                newFire.transform.localScale = Vector3.one * replacementScale;

                SpellMenu.ActiveFires.Add(newFire);
            }
        }
    }
}