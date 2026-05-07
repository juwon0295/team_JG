using UnityEngine;

public class POSMachine : MonoBehaviour
{
    [Header("상호작용 설정")]
    public float interactRange = 2.5f;      // 상호작용 가능 거리
    public POSUIManager posUI;              // POSUIManager 연결

    private Transform player;              // 플레이어 Transform
    private bool isPlayerNear = false;     // 플레이어 근처 여부

    void Start()
    {
        // "Player" 태그를 가진 오브젝트를 자동으로 찾음
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // 플레이어와 포스기 거리 계산
        float distance = Vector3.Distance(transform.position, player.position);
        isPlayerNear = distance <= interactRange;

        // 가까이 있을 때 E키 누르면 UI 열기
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            posUI.OpenPOS();
        }
    }

    // 유니티 Scene 뷰에서 상호작용 범위를 노란 원으로 표시 (디버그용)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
