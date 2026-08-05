using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HandSlot : MonoBehaviour, IDropHandler
{
    [Header("Hand Slot")]
    [SerializeField] private float placedIconSize = 150f;
    [SerializeField] private GameObject instructionText;

    [Header("Encounter")]
    [SerializeField] private EncounterManager encounterManager;

    private DraggableItem currentItem;

    public bool HasItem => currentItem != null;

    public ItemData CurrentItemData =>
        currentItem != null ? currentItem.Data : null;

    public void OnDrop(PointerEventData eventData)
    {
        if (HasItem)
            return;

        if (encounterManager == null)
        {
            Debug.LogError(
                "EncounterManager is not assigned to HandSlot.");
            return;
        }

        if (!encounterManager.CanReceiveItem)
            return;

        if (GameManager.Instance != null &&
            !GameManager.Instance.CanChooseItem)
        {
            return;
        }

        if (eventData.pointerDrag == null)
            return;

        if (!eventData.pointerDrag.TryGetComponent(
                out DraggableItem draggedItem))
        {
            return;
        }

        if (draggedItem.Data == null)
        {
            Debug.LogWarning(
                "The dragged item has no ItemData.");
            return;
        }

        AcceptItem(draggedItem);
    }

    private void AcceptItem(DraggableItem draggedItem)
    {
        currentItem = draggedItem;

        RectTransform handRect =
            GetComponent<RectTransform>();

        currentItem.PlaceInHand(
            handRect,
            placedIconSize);

        if (instructionText != null)
        {
            instructionText.SetActive(false);
        }

        encounterManager.ResolveItem(
            currentItem.Data);
    }

    public void ClearHand()
    {
        if (currentItem != null)
        {
            currentItem.ReturnToOriginalSlot();
        }

        currentItem = null;

        if (instructionText != null)
        {
            instructionText.SetActive(true);
        }
    }
}