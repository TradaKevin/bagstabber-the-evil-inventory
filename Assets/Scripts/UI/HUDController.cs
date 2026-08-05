using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Image healthFill;
    [SerializeField] private TMP_Text healthText;

    [Header("Suspicion")]
    [SerializeField] private Image suspicionFill;
    [SerializeField] private TMP_Text suspicionText;

    [Header("Scores")]
    [SerializeField] private TMP_Text evilText;
    [SerializeField] private TMP_Text goldText;

    public void RefreshHUD(
        int health,
        int maxHealth,
        int suspicion,
        int maxSuspicion,
        int evil,
        int gold)
    {
        float healthPercent =
            (float)health / maxHealth;

        float suspicionPercent =
            (float)suspicion / maxSuspicion;

        UpdateBar(healthFill, healthPercent);
        UpdateBar(suspicionFill, suspicionPercent);

        if (healthText != null)
        {
            healthText.text =
                $"HEALTH  {health} / {maxHealth}";
        }

        if (suspicionText != null)
        {
            suspicionText.text =
                $"SUSPICION  {suspicion} / {maxSuspicion}";
        }

        if (evilText != null)
        {
            evilText.text = $"EVIL: {evil}";
        }

        if (goldText != null)
        {
            goldText.text = $"GOLD: {gold}";
        }
    }

    private void UpdateBar(Image bar, float percentage)
    {
        if (bar == null)
            return;

        percentage = Mathf.Clamp01(percentage);

        // Use Fill Amount when the Image is configured as Filled.
        if (bar.sprite != null &&
            bar.type == Image.Type.Filled)
        {
            bar.fillAmount = percentage;
        }
        else
        {
            // Fallback for Images with Source Image set to None.
            Vector3 newScale =
                bar.rectTransform.localScale;

            newScale.x = percentage;
            bar.rectTransform.localScale = newScale;
        }
    }
}