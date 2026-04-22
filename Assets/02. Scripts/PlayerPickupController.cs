using UnityEngine;

public class PlayerPickupController : MonoBehaviour
{
    [Header("Raycast 설정")]
    public float pickupRange = 3f;         // 조준 가능한 최대 거리
    public LayerMask pickupLayer;          // 집을 수 있는 오브젝트 레이어

    [Header("손 위치 설정")]
    public Transform holdPoint;            // 카메라 앞 손 위치 (빈 오브젝트)

    [Header("UI 안내 텍스트 (선택)")]
    public GameObject pickupUI;            // "E: 집기" 안내 UI (없으면 비워도 됨)
    public GameObject placeUI;             // "E: 내려놓기" 안내 UI (없으면 비워도 됨)

    private PickupObject currentHeldObject = null;  // 현재 손에 든 물건
    private PickupObject aimingAtObject = null;      // 현재 조준 중인 물건
    private bool isAimingAtPlaceTarget = false;      // 지정 위치를 조준 중인지 여부

    void Update()
    {
        HandleRaycast();   // 매 프레임 조준 감지
        HandleInput();     // E 버튼 입력 처리
        UpdateUI();        // UI 업데이트
    }

    void HandleRaycast()
    {
        // 카메라 정중앙에서 앞으로 레이 발사
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // 아무것도 안 들고 있을 때: 집을 수 있는 물건 탐색
        if (currentHeldObject == null)
        {
            aimingAtObject = null; // 초기화

            if (Physics.Raycast(ray, out hit, pickupRange, pickupLayer))
            {
                // 레이에 닿은 오브젝트에 PickupObject 스크립트가 있는지 확인
                PickupObject po = hit.collider.GetComponent<PickupObject>();
                if (po != null && !po.isHeld)
                {
                    aimingAtObject = po; // 조준 중인 물건 저장
                }
            }
        }
        // 물건을 들고 있을 때: 지정 놓는 위치를 조준 중인지 확인
        else
        {
            isAimingAtPlaceTarget = false;

            if (Physics.Raycast(ray, out hit, pickupRange))
            {
                // 현재 든 물건의 placeTarget(지정 위치)을 조준했는지 확인
                if (hit.collider.transform == currentHeldObject.placeTarget)
                {
                    isAimingAtPlaceTarget = true;
                }
            }
        }
    }

    void HandleInput()
    {
        // E 버튼을 눌렀을 때
        if (Input.GetKeyDown(KeyCode.E))
        {
            // [경우 1] 아무것도 안 들고 있고, 물건을 조준 중 → 집기
            if (currentHeldObject == null && aimingAtObject != null)
            {
                currentHeldObject = aimingAtObject;
                currentHeldObject.PickUp(holdPoint);
                aimingAtObject = null;
            }
            // [경우 2] 물건을 들고 있고, 지정 위치를 조준 중 → 원위치로 돌려놓기
            else if (currentHeldObject != null && isAimingAtPlaceTarget)
            {
                currentHeldObject.PlaceBack();
                currentHeldObject = null;
                isAimingAtPlaceTarget = false;
            }
        }
    }

    void UpdateUI()
    {
        // "E: 집기" UI: 물건을 조준 중일 때만 표시
        if (pickupUI != null)
            pickupUI.SetActive(aimingAtObject != null);

        // "E: 내려놓기" UI: 지정 위치를 조준 중일 때만 표시
        if (placeUI != null)
            placeUI.SetActive(isAimingAtPlaceTarget);
    }
}
