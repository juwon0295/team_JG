using UnityEngine;
using TMPro;

public class PlayerDoorInteraction : MonoBehaviour
{
    [Header("Interact Settings")]
    public float interactDistance = 2.5f;
    public KeyCode interactKey = KeyCode.E;

    [Header("UI (선택사항)")]
    public TextMeshProUGUI interactHintText;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        if (interactHintText != null)
            interactHintText.gameObject.SetActive(false);
    }

    void Update()
    {
        DoorController door = GetLookedDoor();

        if (interactHintText != null)
        {
            if (door != null)
            {
                interactHintText.gameObject.SetActive(true);
                interactHintText.text = door.isOpen ? "[E] 문 닫기" : "[E] 문 열기 (통과 가능)";
            }
            else
            {
                interactHintText.gameObject.SetActive(false);
            }
        }

        if (door != null && Input.GetKeyDown(interactKey))
        {
            door.ToggleDoor();
        }
    }

    DoorController GetLookedDoor()
    {
        if (cam != null)
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                DoorController door = hit.collider.GetComponent<DoorController>();
                if (door != null) return door;
            }
        }

        Collider[] cols = Physics.OverlapSphere(transform.position, interactDistance);
        foreach (Collider col in cols)
        {
            DoorController door = col.GetComponent<DoorController>();
            if (door != null) return door;
        }

        return null;
    }
}