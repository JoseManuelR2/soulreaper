using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Header("Configuración de la puerta")]
    public float openAngle = 100f;
    public float duration = 3.3f;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine currentCoroutine;

    void Awake()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    public void OpenDoor()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(RotateDoor(openRotation));
    }

    public void CloseDoor()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(RotateDoor(closedRotation));
    }

    IEnumerator RotateDoor(Quaternion targetRotation)
    {
        Quaternion startRotation = transform.rotation;
        float time = 0f;

        while (time < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;
    }
}