using UnityEngine;

public class AOEEffect : MonoBehaviour
{
    public float duration = 3f;

    void Start()
    {
        Destroy(gameObject, duration);
    }
}