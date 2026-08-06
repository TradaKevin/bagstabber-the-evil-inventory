using System.Collections;
using TMPro;
using UnityEngine;

public enum EncounterType
{
    Slime,
    SpikeTrap,
    Potion,
    LockedDoor,
    FinalVault
}

public class EncounterManager : MonoBehaviour
{
    [Header("Hero")]
    [SerializeField] private Transform hero;

    [Header("Encounter Objects")]
    [SerializeField] private Transform slime;
    [SerializeField] private Transform spikes;

    [Header("UI")]
    [SerializeField] private GameObject requestBubble;
    [SerializeField] private TMP_Text requestText;
    [SerializeField] private GameObject resultPopup;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private HandSlot handSlot;

    [Header("Inventory Slots")]
    [SerializeField] private GameObject slot1Object;
    [SerializeField] private GameObject slot2Object;
    [SerializeField] private GameObject slot3Object;
    [SerializeField] private GameObject slot4Object;

    [SerializeField] private DraggableItem slot1Item;
    [SerializeField] private DraggableItem slot2Item;

    [Header("Slime Items")]
    [SerializeField] private ItemData ironSword;
    [SerializeField] private ItemData woodenSpoon;

    [Header("Spike Trap Items")]
    [SerializeField] private ItemData steelShield;
    [SerializeField] private ItemData paperShield;

    [Header("Potion Items")]
    [SerializeField] private ItemData healthPotion;
    [SerializeField] private ItemData poisonFlask;

    [Header("Locked Door")]
    [SerializeField] private Transform doorPanel;
    [SerializeField] private float doorOpenHeight = 3f;
    [SerializeField] private float doorOpenDuration = 0.45f;
    private Vector3 doorStartPosition;

    [Header("Key Items")]
    [SerializeField] private ItemData realKey;
    [SerializeField] private ItemData mimicKey;

    [Header("Final Vault")]
    [SerializeField] private Transform backpack;
    [SerializeField] private Transform crown;
    [SerializeField] private ItemData betrayalSeal;
    [SerializeField] private float backpackGrowth = 1.35f;
    [SerializeField] private float betrayalMoveDuration = 0.4f;

    [Header("Timing")]
    [SerializeField] private float movementDuration = 0.2f;
    [SerializeField] private float resultDisplayDuration = 1.5f;

    [Header("Result Colours")]
    [SerializeField]
    private Color safeColour =
        new Color(0.51f, 1f, 0.4f);

    [SerializeField]
    private Color evilColour =
        new Color(0.79f, 0.36f, 1f);

    private EncounterType currentEncounter;

    private bool encounterActive;
    private bool isResolving;

    private Vector3 heroStartPosition;
    private Quaternion heroStartRotation;
    private Vector3 heroStartScale;

    private Vector3 slimeStartPosition;
    private Quaternion slimeStartRotation;
    private Vector3 slimeStartScale;

    private Vector3 spikesStartPosition;

    public bool CanReceiveItem =>
        encounterActive && !isResolving;

    private void Awake()
    {
        encounterActive = false;
        isResolving = false;

        if (requestBubble != null)
            requestBubble.SetActive(false);

        if (resultPopup != null)
            resultPopup.SetActive(false);

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    public void BeginEncounter(
        EncounterType encounterType)
    {
        StopAllCoroutines();

        currentEncounter = encounterType;
        encounterActive = true;
        isResolving = false;

        if (handSlot != null)
        {
            handSlot.ClearHand();
        }

        SaveHeroTransform();

        switch (currentEncounter)
        {
            case EncounterType.Slime:
                PrepareSlimeEncounter();
                break;

            case EncounterType.SpikeTrap:
                PrepareSpikeTrapEncounter();
                break;

            case EncounterType.Potion:
                PreparePotionEncounter();
                break;

            case EncounterType.LockedDoor:
                PrepareLockedDoorEncounter();
                break;

            case EncounterType.FinalVault:
                PrepareFinalVaultEncounter();
                break;
        }

        if (requestBubble != null)
            requestBubble.SetActive(true);

        if (resultPopup != null)
            resultPopup.SetActive(false);

        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetState(
                GameState.WaitingForItem);
        }

        Debug.Log(
            currentEncounter + " Encounter Started");
    }

    private void SaveHeroTransform()
    {
        if (hero == null)
            return;

        heroStartPosition = hero.position;
        heroStartRotation = hero.localRotation;
        heroStartScale = hero.localScale;
    }

    private void PrepareSlimeEncounter()
    {
        if (requestText != null)
        {
            requestText.text = "SWORD!";
        }

        ConfigureInventory(
            ironSword,
            woodenSpoon);

        if (slime != null)
        {
            slime.gameObject.SetActive(true);

            slimeStartPosition = slime.position;
            slimeStartRotation = slime.localRotation;
            slimeStartScale = slime.localScale;
        }
    }

    private void PrepareSpikeTrapEncounter()
    {
        if (requestText != null)
        {
            requestText.text = "SHIELD!";
        }

        ConfigureInventory(
            steelShield,
            paperShield);

        if (spikes != null)
        {
            spikesStartPosition = spikes.position;
        }
    }

    private void PreparePotionEncounter()
    {
        if (requestText != null)
        {
            requestText.text = "POTION!";
        }

        ConfigureInventory(
            healthPotion,
            poisonFlask);
    }

    private void PrepareLockedDoorEncounter()
    {
        if (requestText != null)
        {
            requestText.text = "KEY!";
        }

        ConfigureInventory(
            realKey,
            mimicKey);

        if (doorPanel != null)
        {
            doorStartPosition = doorPanel.position;
        }
    }

    private void PrepareFinalVaultEncounter()
    {
        if (requestText != null)
        {
            requestText.text = "BETRAY!";
        }

        ConfigureFinalInventory();

        if (crown != null)
        {
            crown.gameObject.SetActive(true);
        }
    }

    private void ConfigureFinalInventory()
    {
        if (slot1Object != null)
            slot1Object.SetActive(true);

        if (slot2Object != null)
            slot2Object.SetActive(false);

        if (slot3Object != null)
            slot3Object.SetActive(false);

        if (slot4Object != null)
            slot4Object.SetActive(false);

        if (slot1Item != null)
        {
            slot1Item.SetItemData(
                betrayalSeal);
        }
    }

    private void ConfigureInventory(
        ItemData firstItem,
        ItemData secondItem)
    {
        if (slot1Object != null)
            slot1Object.SetActive(true);

        if (slot2Object != null)
            slot2Object.SetActive(true);

        if (slot3Object != null)
            slot3Object.SetActive(false);

        if (slot4Object != null)
            slot4Object.SetActive(false);

        if (slot1Item != null)
            slot1Item.SetItemData(firstItem);

        if (slot2Item != null)
            slot2Item.SetItemData(secondItem);
    }

    public void ResolveItem(ItemData selectedItem)
    {
        if (selectedItem == null ||
            !CanReceiveItem)
        {
            return;
        }

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
            encounterActive = false;
            isResolving = false;
            yield break;
        }

        switch (currentEncounter)
        {
            case EncounterType.Slime:
                yield return PlaySlimeOutcome(
                    selectedItem);
                break;

            case EncounterType.SpikeTrap:
                yield return PlaySpikeOutcome(
                    selectedItem);
                break;

            case EncounterType.Potion:
                yield return PlayPotionOutcome(
                    selectedItem);
                break;

            case EncounterType.LockedDoor:
                yield return PlayDoorOutcome(
                    selectedItem);
                break;

            case EncounterType.FinalVault:
                yield return PlayFinalBetrayalOutcome();
                CompleteFinalBetrayal();
                yield break;
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
            requestBubble.SetActive(false);

        if (resultPopup != null)
            resultPopup.SetActive(true);

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

    private IEnumerator PlaySlimeOutcome(
        ItemData item)
    {
        if (item.ID == ItemID.IronSword)
        {
            yield return PlaySwordOutcome();
        }
        else
        {
            yield return PlaySpoonOutcome();
        }
    }

    private IEnumerator PlaySwordOutcome()
    {
        if (hero == null || slime == null)
            yield break;

        Vector3 attackPosition =
            heroStartPosition +
            Vector3.right * 1.2f;

        yield return MoveObject(
            hero,
            heroStartPosition,
            attackPosition,
            movementDuration);

        yield return ScaleObject(
            slime,
            slimeStartScale,
            slimeStartScale * 1.2f,
            0.08f);

        yield return ScaleObject(
            slime,
            slime.localScale,
            Vector3.zero,
            0.25f);

        slime.gameObject.SetActive(false);

        yield return MoveObject(
            hero,
            attackPosition,
            heroStartPosition,
            movementDuration);

        RestoreHero();
    }

    private IEnumerator PlaySpoonOutcome()
    {
        if (hero == null || slime == null)
            yield break;

        Vector3 attackPosition =
            slimeStartPosition +
            Vector3.left * 1.2f;

        yield return MoveObject(
            slime,
            slimeStartPosition,
            attackPosition,
            movementDuration);

        hero.position =
            heroStartPosition + Vector3.left * 0.5f;

        hero.localRotation =
            heroStartRotation *
            Quaternion.Euler(0f, 0f, 12f);

        yield return new WaitForSeconds(0.25f);

        RestoreHero();

        yield return MoveObject(
            slime,
            attackPosition,
            slimeStartPosition,
            movementDuration);

        yield return ScaleObject(
            slime,
            slimeStartScale,
            Vector3.zero,
            0.25f);

        slime.gameObject.SetActive(false);
    }

    private IEnumerator PlaySpikeOutcome(
        ItemData item)
    {
        if (item.ID == ItemID.SteelShield)
        {
            yield return PlaySteelShieldOutcome();
        }
        else
        {
            yield return PlayPaperShieldOutcome();
        }
    }

    private IEnumerator PlaySteelShieldOutcome()
    {
        if (spikes == null)
            yield break;

        Vector3 raisedPosition =
            spikesStartPosition + Vector3.up;

        yield return MoveObject(
            spikes,
            spikesStartPosition,
            raisedPosition,
            0.25f);

        if (hero != null)
        {
            hero.localScale =
                new Vector3(
                    heroStartScale.x * 1.05f,
                    heroStartScale.y * 0.95f,
                    heroStartScale.z * 1.05f);
        }

        yield return new WaitForSeconds(0.3f);

        RestoreHero();

        yield return MoveObject(
            spikes,
            raisedPosition,
            spikesStartPosition,
            0.25f);
    }

    private IEnumerator PlayPaperShieldOutcome()
    {
        if (spikes == null)
            yield break;

        Vector3 raisedPosition =
            spikesStartPosition + Vector3.up;

        yield return MoveObject(
            spikes,
            spikesStartPosition,
            raisedPosition,
            0.2f);

        if (hero != null)
        {
            hero.position =
                heroStartPosition +
                Vector3.left * 0.6f;

            hero.localRotation =
                heroStartRotation *
                Quaternion.Euler(0f, 0f, 15f);
        }

        yield return new WaitForSeconds(0.35f);

        RestoreHero();

        yield return MoveObject(
            spikes,
            raisedPosition,
            spikesStartPosition,
            0.25f);
    }

    private IEnumerator PlayPotionOutcome(
    ItemData item)
    {
        if (item.ID == ItemID.HealthPotion)
        {
            yield return PlayHealthPotionOutcome();
        }
        else
        {
            yield return PlayPoisonFlaskOutcome();
        }
    }

    private IEnumerator PlayHealthPotionOutcome()
    {
        if (hero == null)
            yield break;

        Vector3 healedScale =
            new Vector3(
                heroStartScale.x * 1.08f,
                heroStartScale.y * 1.08f,
                heroStartScale.z * 1.08f);

        Vector3 raisedPosition =
            heroStartPosition + Vector3.up * 0.25f;

        // Hero grows slightly.
        yield return ScaleObject(
            hero,
            heroStartScale,
            healedScale,
            0.15f);

        // Hero jumps slightly.
        yield return MoveObject(
            hero,
            heroStartPosition,
            raisedPosition,
            0.15f);

        yield return new WaitForSeconds(0.2f);

        yield return MoveObject(
            hero,
            raisedPosition,
            heroStartPosition,
            0.15f);

        yield return ScaleObject(
            hero,
            healedScale,
            heroStartScale,
            0.15f);

        RestoreHero();
    }

    private IEnumerator PlayPoisonFlaskOutcome()
    {
        if (hero == null)
            yield break;

        float timer = 0f;
        float shakeDuration = 0.65f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            float wave =
                Mathf.Sin(timer * 35f);

            hero.position =
                heroStartPosition +
                Vector3.right * wave * 0.12f;

            hero.localRotation =
                heroStartRotation *
                Quaternion.Euler(
                    0f,
                    0f,
                    wave * 10f);

            yield return null;
        }

        RestoreHero();
    }

    private IEnumerator PlayDoorOutcome(
    ItemData item)
    {
        if (item.ID == ItemID.RealKey)
        {
            yield return PlayRealKeyOutcome();
        }
        else
        {
            yield return PlayMimicKeyOutcome();
        }
    }

    private IEnumerator PlayRealKeyOutcome()
    {
        if (doorPanel == null)
            yield break;

        Vector3 openedPosition =
            doorStartPosition +
            Vector3.up * doorOpenHeight;

        // Small happy movement from the hero.
        if (hero != null)
        {
            Vector3 raisedPosition =
                heroStartPosition +
                Vector3.up * 0.15f;

            yield return MoveObject(
                hero,
                heroStartPosition,
                raisedPosition,
                0.1f);

            yield return MoveObject(
                hero,
                raisedPosition,
                heroStartPosition,
                0.1f);
        }

        // Door moves upward.
        yield return MoveObject(
            doorPanel,
            doorStartPosition,
            openedPosition,
            doorOpenDuration);

        RestoreHero();
    }

    private IEnumerator PlayMimicKeyOutcome()
    {
        // Mimic bites the hero.
        if (hero != null)
        {
            float timer = 0f;
            float duration = 0.65f;

            while (timer < duration)
            {
                timer += Time.deltaTime;

                float wave =
                    Mathf.Sin(timer * 40f);

                hero.position =
                    heroStartPosition +
                    Vector3.right * wave * 0.15f;

                hero.localRotation =
                    heroStartRotation *
                    Quaternion.Euler(
                        0f,
                        0f,
                        wave * 12f);

                yield return null;
            }

            RestoreHero();
        }

        // The mimic also breaks the lock,
        // allowing the game to continue.
        if (doorPanel != null)
        {
            Vector3 openedPosition =
                doorStartPosition +
                Vector3.up * doorOpenHeight;

            yield return MoveObject(
                doorPanel,
                doorStartPosition,
                openedPosition,
                doorOpenDuration);
        }
    }

    private IEnumerator PlayFinalBetrayalOutcome()
    {
        Vector3 originalBackpackScale = Vector3.one;

        // Detach the backpack from the hero.
        if (backpack != null)
        {
            backpack.SetParent(null, true);

            originalBackpackScale =
                backpack.localScale;

            Vector3 evilScale =
                originalBackpackScale *
                backpackGrowth;

            yield return ScaleObject(
                backpack,
                originalBackpackScale,
                evilScale,
                0.25f);
        }

        // Backpack moves toward the Crown.
        if (backpack != null && crown != null)
        {
            Vector3 crownApproachPosition =
                crown.position;

            crownApproachPosition.y =
                backpack.position.y;

            crownApproachPosition.x -= 0.4f;

            yield return MoveObject(
                backpack,
                backpack.position,
                crownApproachPosition,
                betrayalMoveDuration);

            // Crown moves onto the backpack.
            Vector3 crownTarget =
                backpack.position +
                Vector3.up * 0.8f;

            yield return MoveObject(
                crown,
                crown.position,
                crownTarget,
                0.2f);

            crown.SetParent(
                backpack,
                true);
        }

        // Backpack escapes to the other side of the gate.
        if (backpack != null)
        {
            Vector3 escapePosition =
                backpack.position;

            escapePosition.x =
                doorStartPosition.x - 1.2f;

            yield return MoveObject(
                backpack,
                backpack.position,
                escapePosition,
                betrayalMoveDuration);
        }

        // Gate drops and traps the hero.
        if (doorPanel != null)
        {
            yield return MoveObject(
                doorPanel,
                doorPanel.position,
                doorStartPosition,
                doorOpenDuration);
        }

        // Hero reacts behind the closed gate.
        if (hero != null)
        {
            float timer = 0f;
            float reactionDuration = 0.7f;

            while (timer < reactionDuration)
            {
                timer += Time.deltaTime;

                float wave =
                    Mathf.Sin(timer * 30f);

                hero.position =
                    heroStartPosition +
                    Vector3.right * wave * 0.12f;

                hero.localRotation =
                    heroStartRotation *
                    Quaternion.Euler(
                        0f,
                        0f,
                        wave * 8f);

                yield return null;
            }

            RestoreHero();
        }

        yield return new WaitForSeconds(0.4f);
    }

    private void CompleteFinalBetrayal()
    {
        encounterActive = false;
        isResolving = false;

        if (handSlot != null)
        {
            handSlot.ClearHand();
        }

        if (requestBubble != null)
        {
            requestBubble.SetActive(false);
        }

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        if (resultPopup != null)
        {
            resultPopup.SetActive(false);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.WinGame();
        }

        Debug.Log(
            "FINAL BETRAYAL COMPLETE!");
    }

    private void RestoreHero()
    {
        if (hero == null)
            return;

        hero.position = heroStartPosition;
        hero.localRotation = heroStartRotation;
        hero.localScale = heroStartScale;
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
        encounterActive = false;
        isResolving = false;

        if (handSlot != null)
            handSlot.ClearHand();

        if (requestBubble != null)
            requestBubble.SetActive(false);

        if (resultPopup != null)
            resultPopup.SetActive(false);

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetState(
                GameState.Walking);
        }

        Debug.Log(
            currentEncounter +
            " Encounter Complete");
    }
}