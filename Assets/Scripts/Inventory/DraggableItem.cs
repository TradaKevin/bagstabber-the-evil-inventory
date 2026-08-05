using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CanvasGroup))]
public class DraggableItem : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("Item")]
    [SerializeField] private ItemData itemData;

    [Header("UI")]
    [SerializeField] private TMP_Text itemNameText;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas rootCanvas;

    private Transform originalParent;
    private int originalSiblingIndex;

    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private Vector2 originalPivot;
    private Vector2 originalSizeDelta;
    private Vector2 originalAnchoredPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;

    private bool dragStarted;

    public ItemData Data => itemData;
    public bool IsPlacedInHand { get; private set; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        Canvas parentCanvas = GetComponentInParent<Canvas>();

        if (parentCanvas != null)
        {
            rootCanvas = parentCanvas.rootCanvas;
        }

        SaveOriginalTransform();
    }

    private void Start()
    {
        RefreshVisuals();
    }

    private void SaveOriginalTransform()
    {
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();

        originalAnchorMin = rectTransform.anchorMin;
        originalAnchorMax = rectTransform.anchorMax;
        originalPivot = rectTransform.pivot;
        originalSizeDelta = rectTransform.sizeDelta;
        originalAnchoredPosition =
            rectTransform.anchoredPosition;

        originalScale = rectTransform.localScale;
        originalRotation = rectTransform.localRotation;
    }

    public void RefreshVisuals()
    {
        if (itemData == null)
            return;

        Image iconImage = GetComponent<Image>();

        if (itemData.Icon != null)
        {
            iconImage.sprite = itemData.Icon;
        }

        iconImage.color = itemData.IconTint;

        if (itemNameText != null)
        {
            itemNameText.text = itemData.DisplayName;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemData == null || IsPlacedInHand)
            return;

        if (GameManager.Instance != null &&
            !GameManager.Instance.CanChooseItem)
        {
            return;
        }

        if (rootCanvas == null)
        {
            Debug.LogError(
                "DraggableItem could not find its Canvas.");
            return;
        }

        dragStarted = true;

        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;

        transform.SetParent(rootCanvas.transform, true);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragStarted)
            return;

        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragStarted && !IsPlacedInHand)
            return;

        canvasGroup.alpha = 1f;

        if (!IsPlacedInHand)
        {
            ReturnToOriginalSlot();
        }

        dragStarted = false;
    }

    public void PlaceInHand(
        RectTransform handSlot,
        float iconSize)
    {
        IsPlacedInHand = true;
        dragStarted = false;

        transform.SetParent(handSlot, false);
        transform.SetAsLastSibling();

        rectTransform.anchorMin =
            new Vector2(0.5f, 0.5f);

        rectTransform.anchorMax =
            new Vector2(0.5f, 0.5f);

        rectTransform.pivot =
            new Vector2(0.5f, 0.5f);

        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta =
            new Vector2(iconSize, iconSize);

        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = false;
    }

    public void ReturnToOriginalSlot()
    {
        IsPlacedInHand = false;
        dragStarted = false;

        transform.SetParent(originalParent, false);
        transform.SetSiblingIndex(originalSiblingIndex);

        rectTransform.anchorMin = originalAnchorMin;
        rectTransform.anchorMax = originalAnchorMax;
        rectTransform.pivot = originalPivot;
        rectTransform.sizeDelta = originalSizeDelta;
        rectTransform.anchoredPosition =
            originalAnchoredPosition;

        rectTransform.localScale = originalScale;
        rectTransform.localRotation = originalRotation;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}