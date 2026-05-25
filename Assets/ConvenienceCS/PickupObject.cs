using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public bool isHeld = false; // 현재 플레이어가 들고 있는 상태인지 확인

    [Header("이 물건의 지정 위치")]
    public Transform placeTarget; // 이 물건을 내려놓을 위치 (POS, 진열대 등)

    [Header("배치 회전 설정")]
    public Vector3 placeRotation = new Vector3(0f, 90f, 0f);
    // Inspector에서 물건마다 다르게 설정 가능
    // 예:
    // (0, 90, 0) → 옆으로 놓기
    // (0, 0, 0) → 정면
    // (0, 180, 0) → 반대 방향
}
