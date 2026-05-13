using UnityEngine;
using TMPro;

public class TrashPickupSystem : MonoBehaviour
{
    [Header("설정")]
    public float pickupRange = 2.5f;
    public float throwRange = 3.0f;
    public Transform holdPoint;
    public LayerMask trashLayer;
    public LayerMask trashCanLayer;

    [Header("UI")]
    public TextMeshProUGUI promptText;

    private TrashItem heldTrash = null;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        SetPrompt("");
    }

    void Update()
    {
        if (heldTrash == null)
            CheckTrashNearby();
        else
            CheckTrashCanNearby();

        HandleInput();
    }

    void CheckTrashNearby()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, trashLayer);

        if (hits.Length > 0)
        {
            if (hits[0].TryGetComponent(out TrashItem trash) && !trash.isPickedUp)
            {
                SetPrompt($"[F] {trash.trashName} 줍기");
                return;
            }
        }
        SetPrompt("");
    }

    void CheckTrashCanNearby()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, throwRange, trashCanLayer);

        if (hits.Length > 0)
        {
            if (hits[0].TryGetComponent(out TrashCan _))
            {
                SetPrompt("[F] 쓰레기통에 버리기   [Q] 내려놓기");
                return;
            }
        }
        SetPrompt("[Q] 내려놓기");
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (heldTrash == null) TryPickUp();
            else TryThrowInTrashCan();
        }

        if (Input.GetKeyDown(KeyCode.Q) && heldTrash != null)
            DropTrash();
    }

    void TryPickUp()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, trashLayer);

        if (hits.Length > 0)
        {
            if (hits[0].TryGetComponent(out TrashItem trash) && !trash.isPickedUp)
            {
                Debug.Log("쓰레기 집음!");
                heldTrash = trash;
                trash.PickUp(holdPoint);
            }
        }
    }

    void TryThrowInTrashCan()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, throwRange, trashCanLayer);

        if (hits.Length > 0)
        {
            if (hits[0].TryGetComponent(out TrashCan can))
            {
                can.AddTrash(heldTrash);
                heldTrash = null;
                SetPrompt("");
            }
        }
    }

    void DropTrash()
    {
        heldTrash.isPickedUp = false;
        heldTrash.GetComponent<Rigidbody>().isKinematic = false;
        heldTrash.GetComponent<Collider>().enabled = true;
        heldTrash.transform.SetParent(null);
        heldTrash = null;
        SetPrompt("");
    }

    void SetPrompt(string text)
    {
        if (promptText) promptText.text = text;
    }
}
