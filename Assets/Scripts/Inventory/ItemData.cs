using UnityEngine;

public enum ItemID
{
    IronSword,
    WoodenSpoon,
    HealthPotion,
    PoisonFlask,
    SteelShield,
    PaperShield,
    RealKey,
    MimicKey,
    BetrayalSeal
}

public enum ItemCategory
{
    Weapon,
    Tool,
    Potion,
    Shield,
    Key,
    Betrayal
}

[CreateAssetMenu(
    fileName = "New_Item",
    menuName = "BAGSTABBER/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private ItemID itemID;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;
    [SerializeField] private Color iconTint = Color.white;

    [TextArea(2, 4)]
    [SerializeField] private string description;

    [Header("Classification")]
    [SerializeField] private ItemCategory category;
    [SerializeField] private bool isEvilChoice;

    [Header("Stat Changes")]
    [SerializeField] private int healthChange;
    [SerializeField] private int suspicionChange;
    [SerializeField] private int evilChange;
    [SerializeField] private int goldChange;

    [Header("Result")]
    [SerializeField] private string resultTitle;

    [TextArea(2, 4)]
    [SerializeField] private string resultDescription;

    public ItemID ID => itemID;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public Color IconTint => iconTint;
    public string Description => description;

    public ItemCategory Category => category;
    public bool IsEvilChoice => isEvilChoice;

    public int HealthChange => healthChange;
    public int SuspicionChange => suspicionChange;
    public int EvilChange => evilChange;
    public int GoldChange => goldChange;

    public string ResultTitle => resultTitle;
    public string ResultDescription => resultDescription;
}