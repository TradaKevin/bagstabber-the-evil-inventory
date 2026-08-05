using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    [Header("HUD")]
    [SerializeField] private HUDController hudController;

    [Header("Maximum Values")]
    [SerializeField, Min(1)] private int maxHealth = 100;
    [SerializeField, Min(1)] private int maxSuspicion = 100;

    public int Health { get; private set; }
    public int Suspicion { get; private set; }
    public int Evil { get; private set; }
    public int Gold { get; private set; }

    public int MaxHealth => maxHealth;
    public int MaxSuspicion => maxSuspicion;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ResetStats();
    }

    public void ApplyChoice(
        int healthChange,
        int suspicionChange,
        int evilChange,
        int goldChange = 0)
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver)
        {
            return;
        }

        Health = Mathf.Clamp(
            Health + healthChange,
            0,
            maxHealth);

        Suspicion = Mathf.Clamp(
            Suspicion + suspicionChange,
            0,
            maxSuspicion);

        Evil = Mathf.Max(
            0,
            Evil + evilChange);

        Gold = Mathf.Max(
            0,
            Gold + goldChange);

        RefreshHUD();
        CheckLoseConditions();
    }

    public void ResetStats()
    {
        Health = maxHealth;
        Suspicion = 0;
        Evil = 0;
        Gold = 0;

        RefreshHUD();
    }

    private void RefreshHUD()
    {
        if (hudController == null)
            return;

        hudController.RefreshHUD(
            Health,
            maxHealth,
            Suspicion,
            maxSuspicion,
            Evil,
            Gold);
    }

    private void CheckLoseConditions()
    {
        if (GameManager.Instance == null)
            return;

        if (Health <= 0)
        {
            GameManager.Instance.LoseGame(
                "THE HERO DIED!\nYOUR EVIL PLAN FAILED.");
        }
        else if (Suspicion >= maxSuspicion)
        {
            GameManager.Instance.LoseGame(
                "THE HERO DISCOVERED YOUR TRUE IDENTITY!");
        }
    }

    [ContextMenu("TEST - Spoon Choice")]
    private void TestSpoonChoice()
    {
        if (!Application.isPlaying)
            return;

        ApplyChoice(
            healthChange: -12,
            suspicionChange: 10,
            evilChange: 15);
    }

    [ContextMenu("TEST - Poison Choice")]
    private void TestPoisonChoice()
    {
        if (!Application.isPlaying)
            return;

        ApplyChoice(
            healthChange: -25,
            suspicionChange: 25,
            evilChange: 30);
    }

    [ContextMenu("TEST - Health Potion")]
    private void TestHealthPotion()
    {
        if (!Application.isPlaying)
            return;

        ApplyChoice(
            healthChange: 30,
            suspicionChange: -5,
            evilChange: 0);
    }

    [ContextMenu("TEST - Reset Everything")]
    private void TestResetEverything()
    {
        if (!Application.isPlaying)
            return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetPrototype();
        }

        ResetStats();
    }
}