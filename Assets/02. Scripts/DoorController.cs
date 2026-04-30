using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 3f;
    public bool isOpen = false;
    public bool isLocked = false;

    [Header("Interact Settings")]
    public float interactDistance = 2.5f;
    public float npcDetectDistance = 3f;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isAnimating = false;
    private bool npcOpened = false;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    void Update()
    {
        // NPC 감지
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        bool npcNearby = false;

        foreach (GameObject npc in npcs)
        {
            float dist = Vector3.Distance(transform.position, npc.transform.position);
            if (dist < npcDetectDistance)
            {
                npcNearby = true;
                break;
            }
        }

        // NPC가 가까워지면 자동으로 열기
        if (npcNearby && !isOpen)
        {
            npcOpened = true;
            ToggleDoor();
        }
        // NPC가 멀어지고 NPC가 열었을 때만 자동으로 닫기
        else if (!npcNearby && isOpen && npcOpened)
        {
            npcOpened = false;
            ToggleDoor();
        }

        // 문 회전 애니메이션
        if (isAnimating)
        {
            Quaternion targetRotation = isOpen ? openRotation : closedRotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.5f)
            {
                transform.rotation = targetRotation;
                isAnimating = false;
            }
        }
    }

    public void ToggleDoor()
    {
        if (isLocked) return;
        isOpen = !isOpen;
        npcOpened = false;
        isAnimating = true;
    }

    public float GetInteractDistance() => interactDistance;
}
