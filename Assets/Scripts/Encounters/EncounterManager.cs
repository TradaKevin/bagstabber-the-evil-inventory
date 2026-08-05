using System.Collections;
using TMPro;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    [Header("Characters")]
    [SerializeField] private Transform hero;
    [SerializeField] private Transform slime;

    [Header("UI")]
    [SerializeField] private GameObject requestBubble;
    [SerializeField] private TMP_Text requestText;
    [SerializeField] private GameObject resultPopup;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private HandSlot handSlot;

    [Header("Encounter")]
    [SerializeField] private string heroRequest = "SWORD!";
    [SerializeField] private float resultDisplayDuration = 1.5f;

    [Header("Animation")]
    [SerializeField] private float heroAttackDistance = 1.2f;
    [SerializeField] private float slimeAttackDistance = 1.2f;
    [SerializeField] private float movementDuration = 0.2f;

    [Header("Result Colours")]
    [SerializeField]
    private Color safeColour =
        new Color(0.51f, 1f, 0.4f);

    [SerializeField]
    private Color evilColour =
        new Color(0.79f, 0.36f, 1f);

    private Vector3 heroStartPosition;
    private Quaternion heroStartRotation;
    private Vector3 heroStartScale;

    private Vector3 slimeStartPosition;
    private Quaternion slimeStartRotation;
    private Vector3 slimeStartScale;

    private bool isResolving;

    public bool CanReceiveItem => !isResolving;

    private void Awake()
    {
        SaveStartingTransforms();
    }

    private void Start()
    {
        BeginSlimeEncounter();
    }

    private void SaveStartingTransforms()
    {
        if (hero != null)
        {
            heroStartPosition = hero.position;
            heroStartRotation = hero.localRotation;
            heroStartScale = hero.localScale;
        }

        if (slime != null)
        {
            slimeStartPosition = slime.position;
            slimeStartRotation = slime.localRotation;
            slimeStartScale = slime.localScale;
        }
    }

    public void BeginSlimeEncounter()
    {
        StopAllCoroutines();

        isResolving = false;

        if (handSlot != null)
        {
            handSlot.ClearHand();
        }

        RestoreCharacters();

        if (requestBubble != null)
        {
            requestBubble.SetActive(true);
        }

        if (requestText != null)
        {
            requestText.text = heroRequest;
        }

        if (resultPopup != null)
        {
            resultPopup.SetActive(false);
        }

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetState(
                GameState.WaitingForItem);
        }

        Debug.Log("Slime Encounter Started");
    }

    public void ResolveItem(ItemData selectedItem)
    {
        if (selectedItem == null || isResolving)
            return;

        StartCoroutine(
            ResolveItemRoutine(selectedItem));
    }

    private IEnumerator ResolveItemRoutine(
        ItemData selectedItem)
    {
        isResolving = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetState(
                GameState.ResolvingItem);
        }

        ApplyItemStats(selectedItem);
        ShowResult(selectedItem);

        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver)
        {
            isResolving = false;
            yield break;
        }

        switch (selectedItem.ID)
        {
            case ItemID.IronSword:
                yield return PlaySwordOutcome();
                break;

            case ItemID.WoodenSpoon:
                yield return PlaySpoonOutcome();
                break;

            default:
                yield return PlayGenericOutcome();
                break;
        }

        yield return new WaitForSeconds(
            resultDisplayDuration);

        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver)
        {
            yield break;
        }

        CompleteEncounter();
    }

    private void ApplyItemStats(ItemData item)
    {
        if (StatsManager.Instance == null)
        {
            Debug.LogWarning(
                "StatsManager was not found.");
            return;
        }

        StatsManager.Instance.ApplyChoice(
            item.HealthChange,
            item.SuspicionChange,
            item.EvilChange,
            item.GoldChange);
    }

    private void ShowResult(ItemData item)
    {
        if (requestBubble != null)
        {
            requestBubble.SetActive(false);
        }

        if (resultPopup != null)
        {
            resultPopup.SetActive(true);
        }

        if (resultText != null)
        {
            resultText.text = item.ResultTitle;

            resultText.color = item.IsEvilChoice
                ? evilColour
                : safeColour;
        }

        Debug.Log(
            item.DisplayName + ": " +
            item.ResultDescription);
    }

    private IEnumerator PlaySwordOutcome()
    {
        if (hero == null || slime == null)
            yield break;

        Vector3 attackPosition =
            heroStartPosition +
            Vector3.right * heroAttackDistance;

        // Hero quickly moves toward the slime.
        yield return MoveObject(
            hero,
            heroStartPosition,
            attackPosition,
            movementDuration);

        // Slime briefly grows when hit.
        yield return ScaleObject(
            slime,
            slimeStartScale,
            slimeStartScale * 1.2f,
            0.08f);

        // Slime disappears.
        yield return ScaleObject(
            slime,
            slime.localScale,
            Vector3.zero,
            0.25f);

        slime.gameObject.SetActive(false);

        // Hero returns to the starting position.
        yield return MoveObject(
            hero,
            attackPosition,
            heroStartPosition,
            movementDuration);

        hero.position = heroStartPosition;
    }

    private IEnumerator PlaySpoonOutcome()
    {
        if (hero == null || slime == null)
            yield break;

        Vector3 slimeAttackPosition =
            slimeStartPosition +
            Vector3.left * slimeAttackDistance;

        // Slime attacks the hero.
        yield return MoveObject(
            slime,
            slimeStartPosition,
            slimeAttackPosition,
            movementDuration);

        // Hero is knocked backwards.
        hero.position =
            heroStartPosition + Vector3.left * 0.5f;

        hero.localRotation =
            heroStartRotation *
            Quaternion.Euler(0f, 0f, 12f);

        yield return new WaitForSeconds(0.25f);

        // Hero recovers.
        hero.position = heroStartPosition;
        hero.localRotation = heroStartRotation;

        // Slime returns.
        yield return MoveObject(
            slime,
            slimeAttackPosition,
            slimeStartPosition,
            movementDuration);

        // Slime leaves after attacking.
        yield return ScaleObject(
            slime,
            slimeStartScale,
            Vector3.zero,
            0.25f);

        slime.gameObject.SetActive(false);
    }

    private IEnumerator PlayGenericOutcome()
    {
        if (hero == null)
            yield break;

        Vector3 raisedPosition =
            heroStartPosition + Vector3.up * 0.25f;

        yield return MoveObject(
            hero,
            heroStartPosition,
            raisedPosition,
            0.15f);

        yield return MoveObject(
            hero,
            raisedPosition,
            heroStartPosition,
            0.15f);

        if (slime != null)
        {
            yield return ScaleObject(
                slime,
                slimeStartScale,
                Vector3.zero,
                0.25f);

            slime.gameObject.SetActive(false);
        }
    }

    private IEnumerator MoveObject(
        Transform objectToMove,
        Vector3 start,
        Vector3 target,
        float duration)
    {
        if (objectToMove == null)
            yield break;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float percentage =
                Mathf.Clamp01(timer / duration);

            objectToMove.position =
                Vector3.Lerp(
                    start,
                    target,
                    percentage);

            yield return null;
        }

        objectToMove.position = target;
    }

    private IEnumerator ScaleObject(
        Transform objectToScale,
        Vector3 start,
        Vector3 target,
        float duration)
    {
        if (objectToScale == null)
            yield break;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float percentage =
                Mathf.Clamp01(timer / duration);

            objectToScale.localScale =
                Vector3.Lerp(
                    start,
                    target,
                    percentage);

            yield return null;
        }

        objectToScale.localScale = target;
    }

    private void CompleteEncounter()
    {
        isResolving = false;

        if (handSlot != null)
        {
            handSlot.ClearHand();
        }

        if (requestBubble != null)
        {
            requestBubble.SetActive(false);
        }

        if (resultPopup != null)
        {
            resultPopup.SetActive(false);
        }

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetState(
                GameState.Walking);
        }

        Debug.Log("Slime Encounter Complete");
    }

    private void RestoreCharacters()
    {
        if (hero != null)
        {
            hero.gameObject.SetActive(true);
            hero.position = heroStartPosition;
            hero.localRotation = heroStartRotation;
            hero.localScale = heroStartScale;
        }

        if (slime != null)
        {
            slime.gameObject.SetActive(true);
            slime.position = slimeStartPosition;
            slime.localRotation = slimeStartRotation;
            slime.localScale = slimeStartScale;
        }
    }

    [ContextMenu("TEST - Reset Slime Encounter")]
    private void TestResetSlimeEncounter()
    {
        if (!Application.isPlaying)
            return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetPrototype();
        }

        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.ResetStats();
        }

        BeginSlimeEncounter();
    }
}