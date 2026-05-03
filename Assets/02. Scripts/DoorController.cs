using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 3f;
    public bool isOpen = false;
    public bool isLocked = false;

    [Header("NPC Settings")]
    public float npcDetectDistance = 3f;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isAnimating = false;
    private bool npcOpened = false;
    private Collider doorCollider;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
        doorCollider = GetComponent<Collider>();
    }

    void Update()
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        bool npcNearby = false;

        foreach (GameObject npc in npcs)
        {
            if (Vector3.Distance(transform.position, npc.transform.position) < npcDetectDistance)
            {
                npcNearby = true;
                break;
            }
        }

        if (npcNearby && !isOpen) { npcOpened = true; ToggleDoor(); }
        else if (!npcNearby && isOpen && npcOpened) { npcOpened = false; ToggleDoor(); }

        if (isAnimating)
        {
            Quaternion target = isOpen ? openRotation : closedRotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * openSpeed);

            if (Quaternion.Angle(transform.rotation, target) < 0.5f)
            {
                transform.rotation = target;
                isAnimating = false;
            }
        }

        if (doorCollider != null)
            doorCollider.enabled = !isOpen;
    }

    public void ToggleDoor()
    {
        if (isLocked) return;
        isOpen = !isOpen;
        isAnimating = true;
    }
}
