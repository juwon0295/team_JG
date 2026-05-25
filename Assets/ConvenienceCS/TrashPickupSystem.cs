using UnityEngine;
using TMPro;

public class TrashPickupSystem : MonoBehaviour
{
    [Header("설정")]
    public float pickupRange = 2.5f;        // 쓰레기를 집을 수 있는 최대 거리
    public float throwRange = 3.0f;         // 쓰레기통에 버릴 수 있는 최대 거리
    public Transform holdPoint;             // 쓰레기가 붙을 위치 (카메라 자식이어야 함)
    public LayerMask trashLayer;            // 쓰레기 전용 레이어
    public LayerMask trashCanLayer;         // 쓰레기통 전용 레이어

    [Header("UI")]
    public TextMeshProUGUI promptText;      // 상황별 안내 텍스트

    private TrashItem heldTrash = null;     // 현재 들고 있는 쓰레기
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        SetPrompt("");  // 시작 시 안내 텍스트 비우기
    }

    void Update()
    {
        // 쓰레기를 들고 있지 않을 때
        if (heldTrash == null)
        {
            // 손님이 매장에 있으면 줍기 차단
            if (IsCustomerPresent())
            {
                // 쓰레기를 조준 중일 때만 안내 텍스트 표시
                if (IsAimingAtTrash())
                    SetPrompt("손님응대를 먼저하세요");
                else
                    SetPrompt("");

                return;
            }

            SetPrompt(""); // 손님 없으면 안내 텍스트 비우기
            CheckTrashAiming();
        }
        else
        {
            // 쓰레기 들고 있을 때는 손님 유무 상관없이 마무리 가능
            CheckTrashCanAiming();
        }

        HandleInput();
    }

    // ── 씬에 손님(NpcController)이 있는지 확인 ──────────────
    bool IsCustomerPresent()
    {
        // 씬에 NpcController가 하나라도 있으면 손님이 매장에 있는 것
        return FindAnyObjectByType<NpcController>() != null;
    }

    // ── 조준점이 쓰레기를 향하는지 확인 ─────────────────────
    bool IsAimingAtTrash()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        return Physics.Raycast(ray, pickupRange, trashLayer);
    }

    // ── 조준 중인 쓰레기 감지 (안 들고 있을 때) ─────────────
    void CheckTrashAiming()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, trashLayer))
        {
            if (hit.collider.TryGetComponent(out TrashItem trash) && !trash.isPickedUp)
            {
                SetPrompt($"[E] {trash.trashName} 줍기");
                return;
            }
        }

        SetPrompt("");
    }

    // ── 조준 중인 쓰레기통 감지 (들고 있을 때) ──────────────
    void CheckTrashCanAiming()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, throwRange, trashCanLayer))
        {
            if (hit.collider.TryGetComponent(out TrashCan _))
            {
                SetPrompt("[E] 쓰레기통에 버리기");
                return;
            }
        }

        SetPrompt("");
    }

    // ── 키 입력 처리 ─────────────────────────────────────────
    void HandleInput()
    {
        // E키: 쓰레기 줍기 or 쓰레기통에 버리기
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldTrash == null)
                TryPickUp();
            else
                TryThrowInTrashCan();
        }
    }

    // ── 쓰레기 줍기 ──────────────────────────────────────────
    void TryPickUp()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, trashLayer))
        {
            if (hit.collider.TryGetComponent(out TrashItem trash) && !trash.isPickedUp)
            {
                heldTrash = trash;
                trash.PickUp(holdPoint);
            }
        }
    }

    // ── 쓰레기통에 버리기 ────────────────────────────────────
    void TryThrowInTrashCan()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, throwRange, trashCanLayer))
        {
            if (hit.collider.TryGetComponent(out TrashCan can))
            {
                // 쓰레기통에 버리기 (TrashCan.AddTrash() → TrashItem.Dispose() → Destroy)
                can.AddTrash(heldTrash);
                heldTrash = null;
                SetPrompt("");

                // 씬에 남은 쓰레기가 있는지 확인
                TrashItem[] remainingTrash = FindObjectsByType<TrashItem>(FindObjectsSortMode.None);

                // 씬에 쓰레기가 하나도 없으면 미션 완료
                if (remainingTrash.Length == 0)
                {
                    StoreMissionManager.Instance.CompleteTrash();
                }
            }
        }
    }

    // ── 안내 텍스트 설정 ─────────────────────────────────────
    void SetPrompt(string text)
    {
        if (promptText) promptText.text = text;
    }
}
