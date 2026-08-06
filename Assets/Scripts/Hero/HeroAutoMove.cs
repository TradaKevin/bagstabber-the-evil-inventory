using UnityEngine;

public class HeroAutoMove : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] private Transform[] waypoints;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stoppingDistance = 0.05f;

    [Header("Rotation")]
    [SerializeField] private bool rotateTowardsMovement = true;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float rotationOffsetY;

    private int currentWaypointIndex;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    public bool HasFinishedPath =>
        waypoints == null ||
        currentWaypointIndex >= waypoints.Length;

    private void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        // Movement automatically pauses during encounters.
        if (GameManager.Instance.CurrentState !=
            GameState.Walking)
        {
            return;
        }

        if (HasFinishedPath)
            return;

        MoveTowardsCurrentWaypoint();
    }

    private void MoveTowardsCurrentWaypoint()
    {
        Transform targetWaypoint =
            waypoints[currentWaypointIndex];

        if (targetWaypoint == null)
        {
            currentWaypointIndex++;
            return;
        }

        Vector3 direction =
            targetWaypoint.position - transform.position;

        RotateTowardsDirection(direction);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWaypoint.position,
            moveSpeed * Time.deltaTime);

        float distance = Vector3.Distance(
            transform.position,
            targetWaypoint.position);

        if (distance <= stoppingDistance)
        {
            transform.position =
                targetWaypoint.position;

            ReachWaypoint(targetWaypoint);
        }
    }

    private void RotateTowardsDirection(
        Vector3 direction)
    {
        if (!rotateTowardsMovement)
            return;

        Vector3 flatDirection =
            new Vector3(
                direction.x,
                0f,
                direction.z);

        if (flatDirection.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                flatDirection.normalized) *
            Quaternion.Euler(
                0f,
                rotationOffsetY,
                0f);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
    }

    private void ReachWaypoint(
        Transform reachedWaypoint)
    {
        Debug.Log(
            "Hero reached: " +
            reachedWaypoint.name);

        // Move to the next waypoint after this one.
        currentWaypointIndex++;

        if (reachedWaypoint.TryGetComponent(
                out EncounterPoint encounterPoint))
        {
            encounterPoint.TriggerEncounter();
        }

        if (HasFinishedPath)
        {
            Debug.Log(
                "Hero finished the current path.");
        }
    }

    public void ResetPath()
    {
        currentWaypointIndex = 0;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }

    private void OnDrawGizmosSelected()
    {
        if (waypoints == null)
            return;

        Gizmos.color = Color.cyan;

        Vector3 previousPosition =
            transform.position;

        foreach (Transform waypoint in waypoints)
        {
            if (waypoint == null)
                continue;

            Gizmos.DrawLine(
                previousPosition,
                waypoint.position);

            Gizmos.DrawWireSphere(
                waypoint.position,
                0.2f);

            previousPosition = waypoint.position;
        }
    }
}