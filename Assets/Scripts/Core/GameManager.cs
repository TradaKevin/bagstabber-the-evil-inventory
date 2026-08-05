using TMPro;
using UnityEngine;

public enum GameState
{
    Intro,
    Walking,
    WaitingForItem,
    ResolvingItem,
    FinalBetrayal,
    Won,
    Lost
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField]
    private GameState startingState =
        GameState.WaitingForItem;

    [Header("End Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TMP_Text loseReasonText;

    public GameState CurrentState { get; private set; }

    public bool CanChooseItem =>
        CurrentState == GameState.WaitingForItem;

    public bool IsGameOver =>
        CurrentState == GameState.Won ||
        CurrentState == GameState.Lost;

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
        ResetPrototype();
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log("Game State: " + CurrentState);
    }

    public void WinGame()
    {
        if (IsGameOver)
            return;

        SetState(GameState.Won);

        if (losePanel != null)
            losePanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void LoseGame(string reason)
    {
        if (IsGameOver)
            return;

        SetState(GameState.Lost);

        if (winPanel != null)
            winPanel.SetActive(false);

        if (loseReasonText != null)
            loseReasonText.text = reason;

        if (losePanel != null)
            losePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ResetPrototype()
    {
        Time.timeScale = 1f;

        if (winPanel != null)
            winPanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(false);

        SetState(startingState);
    }
}