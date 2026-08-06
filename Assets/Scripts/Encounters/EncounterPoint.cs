using UnityEngine;

public class EncounterPoint : MonoBehaviour
{
    [Header("Encounter")]
    [SerializeField] private EncounterManager encounterManager;
    [SerializeField] private EncounterType encounterType;
    [SerializeField] private bool triggerOnlyOnce = true;

    private bool hasTriggered;

    public bool TriggerEncounter()
    {
        if (triggerOnlyOnce && hasTriggered)
            return false;

        if (encounterManager == null)
        {
            Debug.LogError(
                "EncounterManager is not assigned to " +
                gameObject.name);

            return false;
        }

        hasTriggered = true;

        encounterManager.BeginEncounter(
            encounterType);

        return true;
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = encounterType ==
            EncounterType.Slime
            ? Color.magenta
            : Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            0.35f);

        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.up);
    }
}