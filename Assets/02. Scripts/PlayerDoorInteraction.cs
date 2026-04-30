using UnityEngine;
using TMPro;

public class PlayerDoorInteraction : MonoBehaviour
{
    [Header("UI (선택사항)")]
    public TextMeshProUGUI interactHintText;

    private DoorController nearestDoor = null;
    private DoorController[] allDoors;

    void Start()
    {
        allDoors = FindObjectsByType<DoorController>(FindObjectsSortMode.None);

        if (interactHintText != null)
            interactHintText.gameObject.SetActive(false);
    }

    void Update()
    {
        FindNearestDoor();

        if (nearestDoor != null && Input.GetKeyDown(KeyCode.E))
        {
            nearestDoor.ToggleDoor();
        }
    }

    void FindNearestDoor()
    {
        DoorController closest = null;
        float closestDist = Mathf.Infinity;

        foreach (DoorController door in allDoors)
        {
            float dist = Vector3.Distance(transform.position, door.transform.position);
            if (dist < door.GetInteractDistance() && dist < closestDist)
            {
                closestDist = dist;
                closest = door;
            }
        }

        nearestDoor = closest;

        if (interactHintText != null)
        {
            if (nearestDoor != null)
            {
                interactHintText.gameObject.SetActive(true);
                interactHintText.text = nearestDoor.isOpen ? "[E] 문 닫기" : "[E] 문 열기";
            }
            else
            {
                interactHintText.gameObject.SetActive(false);
            }
        }
    }
}