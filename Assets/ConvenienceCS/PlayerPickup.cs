using UnityEngine;
using TMPro;

public class PlayerPickup : MonoBehaviour
{
    [Header("조준 설정")]
    public float pickupRange = 3f;          // 플레이어가 물건을 집을 수 있는 최대 거리

    [Header("손 위치")]
    public Transform holdPoint;             // 물건이 붙을 위치 (카메라 자식이어야 함)

    [Header("UI 텍스트")]
    public GameObject pickupUI;             // "E를 눌러 집기" UI
    public GameObject placeUI;              // "E를 눌러 내려놓기" UI
    public TextMeshProUGUI promptText;      // 손님 응대 안내 텍스트

    private PickupObject heldObject = null; // 현재 들고 있는 물건

    void Update()
    {
        // 아무것도 안 들고 있을 때 → 물건 감지
        if (heldObject == null)
        {
            // 손님이 매장에 있으면 집기 차단
            if (IsCustomerPresent())
            {
                // 집기 UI 숨기기
                if (pickupUI != null)
                    pickupUI.SetActive(false);

                // 물건을 조준 중일 때만 안내 텍스트 표시
                if (IsAimingAtPickable())
                    SetPrompt("손님응대를 먼저하세요");
                else
                    SetPrompt("");

                return;
            }

            SetPrompt(""); // 손님 없으면 안내 텍스트 비우기
            CheckAiming();
        }
        else
        {
            // 물건 들고 있을 때는 손님 유무 상관없이 마무리 가능
            SetPrompt("");

            // 집기 UI 끄기
            if (pickupUI != null)
                pickupUI.SetActive(false);

            // 내려놓을 위치 감지
            CheckPlaceAiming();
        }
    }

    // ── 씬에 손님(NpcController)이 있는지 확인 ──────────────
    bool IsCustomerPresent()
    {
        // 씬에 NpcController가 하나라도 있으면 손님이 매장에 있는 것
        return FindAnyObjectByType<NpcController>() != null;
    }

    // ── 조준점이 Pickable 레이어 물건을 향하는지 확인 ────────
    bool IsAimingAtPickable()
    {
        int layerMask = LayerMask.GetMask("Pickable");

        Ray ray = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width / 2, Screen.height / 2)
        );

        return Physics.Raycast(ray, pickupRange, layerMask);
    }

    // ── 조준점 기준으로 집을 수 있는 물건 감지 ───────────────
    void CheckAiming()
    {
        // Pickable 레이어만 감지
        int layerMask = LayerMask.GetMask("Pickable");

        // 화면 중앙 (조준점)에서 Ray 발사
        Ray ray = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width / 2, Screen.height / 2)
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, layerMask))
        {
            PickupObject po = hit.collider.GetComponent<PickupObject>();

            if (po != null)
            {
                // 집기 UI 표시
                if (pickupUI != null)
                    pickupUI.SetActive(true);

                // E키 입력 시 집기
                if (Input.GetKeyDown(KeyCode.E))
                    PickUp(po);
            }
            else
            {
                if (pickupUI != null)
                    pickupUI.SetActive(false);
            }
        }
        else
        {
            if (pickupUI != null)
                pickupUI.SetActive(false);
        }
    }

    // ── 내려놓을 위치(placeTarget)를 조준 중인지 확인 ────────
    void CheckPlaceAiming()
    {
        Ray ray = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width / 2, Screen.height / 2)
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            // 현재 들고 있는 물건의 지정 위치인지 확인
            if (hit.transform == heldObject.placeTarget)
            {
                if (placeUI != null)
                    placeUI.SetActive(true);

                // E키 입력 시 내려놓기
                if (Input.GetKeyDown(KeyCode.E))
                    PlaceObject();
            }
            else
            {
                if (placeUI != null)
                    placeUI.SetActive(false);
            }
        }
        else
        {
            if (placeUI != null)
                placeUI.SetActive(false);
        }
    }

    // ── 물건 집기 ─────────────────────────────────────────────
    void PickUp(PickupObject obj)
    {
        heldObject = obj;
        heldObject.isHeld = true;

        // 물리 비활성화
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // 손에 든 상태에서는 Raycast에서 제외
        heldObject.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        // 손 위치로 이동
        heldObject.transform.SetParent(holdPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;
    }

    // ── 물건 내려놓기 ─────────────────────────────────────────
    void PlaceObject()
    {
        // 부모 해제
        heldObject.transform.SetParent(null);

        // 지정된 위치로 이동
        heldObject.transform.position = heldObject.placeTarget.position;

        // 회전값 적용 (진열 방향 맞추기)
        heldObject.transform.rotation = Quaternion.Euler(heldObject.placeRotation);

        // 물리 다시 활성화
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // 다시 못 집도록 레이어 변경 (Placed)
        heldObject.gameObject.layer = LayerMask.NameToLayer("Placed");

        // UI 숨기기
        if (placeUI != null)
            placeUI.SetActive(false);

        // 상태 초기화
        heldObject.isHeld = false;
        heldObject = null;

        // ── 씬에 아직 집지 않은 물건이 있는지 확인 ──────────
        int pickableLayer = LayerMask.NameToLayer("Pickable");
        PickupObject[] allItems = FindObjectsByType<PickupObject>(FindObjectsSortMode.None);

        bool anyRemaining = false;
        foreach (PickupObject item in allItems)
        {
            if (item.gameObject.layer == pickableLayer)
            {
                anyRemaining = true;
                break;
            }
        }

        // 집어야 할 물건이 모두 배치됐을 때만 미션 완료
        if (!anyRemaining)
        {
            StoreMissionManager.Instance.CompleteRestock();
        }
    }

    // ── 안내 텍스트 설정 ─────────────────────────────────────
    void SetPrompt(string text)
    {
        if (promptText != null) promptText.text = text;
    }
}
