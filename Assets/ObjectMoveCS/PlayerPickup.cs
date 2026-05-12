using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("조준 설정")]
    public float pickupRange = 3f;              // 물건을 집을 수 있는 최대 거리

    [Header("손 위치")]
    public Transform holdPoint;                 // 카메라 앞 빈 오브젝트 (물건이 붙을 위치)

    [Header("UI 텍스트")]
    public GameObject pickupUI;                 // 'E버튼을 눌러서 들기' 텍스트 오브젝트
    public GameObject placeUI;                  // 'E버튼을 눌러서 놓기' 텍스트 오브젝트

    private PickupObject heldObject = null;     // 현재 손에 들고 있는 물건

    void Update()
    {
        // 아무것도 안 들고 있을 때 → 조준 감지
        if (heldObject == null)
        {
            CheckAiming();
        }
        else
        {
            // 물건을 들고 있을 때 → 집기 UI 숨기고 놓을 위치 조준 감지
            if (pickupUI != null)
                pickupUI.SetActive(false);
            CheckPlaceAiming();
        }
    }

    // 카메라 정중앙(조준점)에서 레이캐스트를 쏴서 PickupObject 감지
    void CheckAiming()
    {
        // 카메라 중앙에서 Ray 발사 (조준점 기준)
        int layerMask = LayerMask.GetMask("Pickable");
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, layerMask))
        {
            PickupObject po = hit.collider.GetComponent<PickupObject>();

            if (po != null)
            {
                // 집을 수 있는 물건을 조준 중 → 집기 UI 표시
                if (pickupUI != null)
                    pickupUI.SetActive(true);

                // E키 누르면 집기
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

    // 놓을 위치(placeTarget)를 조준 중인지 감지
    void CheckPlaceAiming()
    {
        // 카메라 중앙에서 Ray 발사 (조준점 기준)
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            // placeTarget과 같은 오브젝트를 바라보고 있는지 체크
            if (hit.transform == heldObject.placeTarget)
            {
                if (placeUI != null)
                    placeUI.SetActive(true);

                // E키 누르면 내려놓기
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

    // 물건을 holdPoint 자식으로 붙이고 물리/콜라이더 비활성화
    void PickUp(PickupObject obj)
    {
        heldObject = obj;
        heldObject.isHeld = true;

        // ✅ Rigidbody 제거하지 말고 물리만 끄기
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Collider는 끄지 말고 Raycast만 무시하도록 레이어 변경
        heldObject.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        // HoldPoint에 붙이기
        heldObject.transform.SetParent(holdPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;
    }


    void PlaceObject()
    {
        heldObject.transform.SetParent(null);

        heldObject.transform.position = heldObject.placeTarget.position;

        // Y축 90도 회전
        heldObject.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // 다시 못 들게 하기
        heldObject.gameObject.layer = LayerMask.NameToLayer("Placed");

        if (placeUI != null)
            placeUI.SetActive(false);

        heldObject.isHeld = false;
        heldObject = null;
    }
}

