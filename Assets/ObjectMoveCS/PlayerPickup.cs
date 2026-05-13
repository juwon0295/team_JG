using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("조준 설정")]
    public float pickupRange = 3f; // 플레이어가 물건을 집을 수 있는 최대 거리

    [Header("손 위치")]
    public Transform holdPoint; // 물건이 붙을 위치 (카메라 자식이어야 함)

    [Header("UI 텍스트")]
    public GameObject pickupUI; // "E를 눌러 집기" UI
    public GameObject placeUI;  // "E를 눌러 내려놓기" UI

    private PickupObject heldObject = null; // 현재 들고 있는 물건

    [Header("배치 설정")]
    public Vector3 placeRotation = new Vector3(0f, 90f, 0f); // 내려놓을 때 회전값

    void Update()
    {
        // 아무것도 안 들고 있을 때 → 물건 감지
        if (heldObject == null)
        {
            CheckAiming();
        }
        else
        {
            // 물건 들고 있을 때는 집기 UI 끄기
            if (pickupUI != null)
                pickupUI.SetActive(false);

            // 내려놓을 위치 감지
            CheckPlaceAiming();
        }
    }

    // 조준점 기준으로 집을 수 있는 물건 감지
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
            // 맞은 오브젝트에 PickupObject가 있는지 확인
            PickupObject po = hit.collider.GetComponent<PickupObject>();

            if (po != null)
            {
                // UI 표시
                if (pickupUI != null)
                    pickupUI.SetActive(true);

                // E키 입력 시 집기
                if (Input.GetKeyDown(KeyCode.E))
                    PickUp(po);
            }
            else
            {
                // 대상이 아니면 UI 숨김
                if (pickupUI != null)
                    pickupUI.SetActive(false);
            }
        }
        else
        {
            // 아무것도 안 맞으면 UI 숨김
            if (pickupUI != null)
                pickupUI.SetActive(false);
        }
    }

    // 내려놓을 위치(placeTarget)를 조준 중인지 확인
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
                // 내려놓기 UI 표시
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

    // 물건 집기
    void PickUp(PickupObject obj)
    {
        heldObject = obj;
        heldObject.isHeld = true;

        // 물리 비활성화 (삭제하지 않고 유지)
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // 물리 영향 제거
            rb.useGravity = false; // 중력 제거
        }

        // Raycast에서 제외 (손에 든 상태에서는 다시 감지 안 되도록)
        heldObject.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        // 손 위치로 이동
        heldObject.transform.SetParent(holdPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;
    }

    // 물건 내려놓기
    void PlaceObject()
    {
        // 부모 해제
        heldObject.transform.SetParent(null);

        // 지정된 위치로 이동
        heldObject.transform.position = heldObject.placeTarget.position;

        // Y축 회전 (진열 방향 맞추기)
        heldObject.transform.rotation = Quaternion.Euler(heldObject.placeRotation);

        // 물리 다시 활성화
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // 다시 못 집도록 레이어 변경
        heldObject.gameObject.layer = LayerMask.NameToLayer("Placed");

        // UI 숨기기
        if (placeUI != null)
            placeUI.SetActive(false);

        // 상태 초기화
        heldObject.isHeld = false;
        heldObject = null;
    }
}
