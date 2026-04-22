using UnityEngine;

public class PickupObject : MonoBehaviour
{
    [Header("원래 위치/회전 (자동 저장됨)")]
    public Vector3 originalPosition;       // 물건의 원래 위치
    public Quaternion originalRotation;    // 물건의 원래 회전값

    [Header("지정된 놓는 위치 (빈 오브젝트로 설정)")]
    public Transform placeTarget;          // 물건을 놓을 지정 위치 (Inspector에서 드래그)

    [HideInInspector]
    public bool isHeld = false;            // 현재 손에 들고 있는지 여부

    void Start()
    {
        // 게임 시작 시 현재 위치/회전을 원래 위치로 저장
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    // 물건을 손에 드는 함수
    public void PickUp(Transform holdPoint)
    {
        isHeld = true;

        // 물건을 holdPoint(손 위치)의 자식으로 설정
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;  // 손 위치에 딱 붙임
        transform.localRotation = Quaternion.identity;

        // 물리 비활성화 (들고 있을 때 중력/충돌 무시)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // 콜라이더 비활성화 (들고 있을 때 다른 오브젝트와 충돌 방지)
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
    }

    // 물건을 원래 위치로 돌려놓는 함수
    public void PlaceBack()
    {
        isHeld = false;

        // 부모 해제 후 원래 위치/회전으로 이동
        transform.SetParent(null);
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        // 물리 다시 활성화
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // 콜라이더 다시 활성화
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
    }
}
