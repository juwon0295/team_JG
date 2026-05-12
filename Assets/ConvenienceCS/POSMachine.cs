using UnityEngine;

public class POSMachine : MonoBehaviour
{
    [Header("상호작용 설정")]
    public float interactRange = 3f;        // 조준점 기준 상호작용 가능 거리
    public POSUIManager posUI;              // POSUIManager 연결

    private Camera playerCamera;           // 플레이어 카메라

    void Start()
    {
        // 메인 카메라를 플레이어 카메라로 사용
        playerCamera = Camera.main;
    }

    void Update()
    {
        // UI가 이미 열려있으면 E키 입력 무시
        if (posUI.isOpen) return;

        // 화면 정중앙(조준점)에서 앞쪽으로 Ray 발사
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        // Ray가 interactRange 거리 안에서 무언가에 맞았을 때
        if (Physics.Raycast(ray, out hit, interactRange))
        {
            // Ray에 맞은 오브젝트가 이 포스기인지 확인
            if (hit.transform == this.transform)
            {
                // E키를 누르면 포스기 UI 열기
                if (Input.GetKeyDown(KeyCode.E))
                {
                    posUI.OpenPOS();
                }
            }
        }
    }

    // Scene 뷰에서 상호작용 범위 시각적으로 표시 (디버그용)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
