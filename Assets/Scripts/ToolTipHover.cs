using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TooltipController : MonoBehaviour
{
    public GameObject tooltipPanel;
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.3f;
    public float scaleDuration = 0.3f;

    private void Start()
    {
        // Default state
        tooltipPanel.SetActive(false);
        canvasGroup.alpha = 0;
        tooltipPanel.transform.localScale = Vector3.zero;
    }

    public void OnHoverEnter()
    {
        // Display Tooltip 
        tooltipPanel.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    public void OnHoverExit()
    {
        // Hide Tooltip
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t);
            tooltipPanel.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t);
            tooltipPanel.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            yield return null;
        }
        tooltipPanel.SetActive(false);
    }
}
