using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("조준 설정")]
    public float pickupRange = 3f;         // 물건을 집을 수 있는 최대 거리

    [Header("손 위치")]
    public Transform holdPoint;            // 카메라 앞 빈 오브젝트 (물건이 붙을 위치)

    [Header("UI 텍스트")]
    public GameObject pickupUI;            // 'E버튼을 눌러서 들기' 텍스트 오브젝트
    public GameObject placeUI;             // 'E버튼을 눌러서 놓기' 텍스트 오브젝트

    private PickupObject heldObject = null; // 현재 손에 들고 있는 물건

    void Update()
    { 
        // 아무것도 안 들고 있을 때 → 조준 감지
        if (heldObject == null) { 
            CheckAiming(); 
        } else { 
            if (pickupUI != null) 
                pickupUI.SetActive(false); // 물건을 들고 있을 때 → 놓을 위치 조준 감지
            CheckPlaceAiming();
        } 
    }

        // 카메라 정중앙에서 레이캐스트를 쏴서 PickupObject가 있는지 감지
        void CheckAiming()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            // 조준된 오브젝트에 PickupObject 스크립트가 있으면 집을 수 있는 물건
            PickupObject po = hit.collider.GetComponent<PickupObject>();

            if (po != null)
            {
                if (pickupUI != null)
                    pickupUI.SetActive(true);

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

    void CheckPlaceAiming()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            // placeTarget과 같은 오브젝트를 바라보고 있는지 체크
            if (hit.transform == heldObject.placeTarget)
            {
                if (placeUI != null)
                    placeUI.SetActive(true);

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

        // 물건을 손 위치에 붙이기
        heldObject.transform.SetParent(holdPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;

        // 들고 있는 동안 물리/충돌 비활성화
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Collider col = heldObject.GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    // 각 물건의 placeTarget 위치에 내려놓고 물리/콜라이더 복원
    void PlaceObject()
    {
        heldObject.transform.SetParent(null);

        // 물건마다 지정된 위치(placeTarget)로 이동
        heldObject.transform.position = heldObject.placeTarget.position;
        heldObject.transform.rotation = Quaternion.identity;

        // 내려놓은 후 물리/충돌 복원
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        Collider col = heldObject.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        // 한 번 놓으면 다시 못 집게 스크립트 제거
        Destroy(heldObject.GetComponent<PickupObject>());   
        if (placeUI != null) placeUI.SetActive(false);      // 놓은 후 UI 숨기기
        heldObject = null;
    }
}
