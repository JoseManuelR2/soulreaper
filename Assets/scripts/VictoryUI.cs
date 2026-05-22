using System.Collections;
using TMPro;
using UnityEngine;

public class VRVictoryUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject victoryUI;

    private TextMeshProUGUI victoryText;

    void Start()
    {
        if (victoryUI != null)
        {
            victoryText = victoryUI.GetComponent<TextMeshProUGUI>();

            Color c = victoryText.color;
            c.a = 0f;
            victoryText.color = c;

            victoryUI.SetActive(false);
        }
    }

    public void ShowVictory()
    {
        StartCoroutine(FadeVictoryText());
    }

    IEnumerator FadeVictoryText()
    {
        if (victoryText == null)
            yield break;

        // Posicionar delante de la cámara
        victoryUI.transform.position =
            transform.position +
            transform.forward * 2f +
            Vector3.down * 0.2f;

        // Mirar hacia el jugador
        victoryUI.transform.rotation =
            Quaternion.LookRotation(
                victoryUI.transform.position - transform.position
            );

        victoryUI.SetActive(true);

        Color color = victoryText.color;

        color.a = 0f;
        victoryText.color = color;

        float duration = 1f;
        float timer = 0f;

        victoryUI.transform.localScale = Vector3.one * 0.8f;

        // Fade In
        while (timer < duration)
        {
            timer += Time.deltaTime;

            victoryUI.transform.localScale =
                Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, timer / duration);

            color.a = Mathf.Lerp(0f, 1f, timer / duration);
            victoryText.color = color;

            yield return null;
        }

        yield return new WaitForSeconds(2f);

        // Fade Out
        timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            victoryUI.transform.localScale =
                Vector3.Lerp(Vector3.one, Vector3.one * 0.8f, timer / duration);

            color.a = Mathf.Lerp(1f, 0f, timer / duration);
            victoryText.color = color;

            yield return null;
        }

        victoryUI.SetActive(false);
    }
}