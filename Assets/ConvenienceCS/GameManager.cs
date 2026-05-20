using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 싱글톤: 다른 스크립트에서 GameManager.Instance로 접근

    [Header("연결")]
    public PlayerController playerController;       // 플레이어 이동 스크립트
    public StoreMissionManager missionManager;      // 미션 UI 스크립트

    // Phase 2 진입 여부 (POSUIManager에서 포스기 닫을 때 이동 허용 여부 판단에 사용)
    public bool IsPhase2 { get; private set; } = false;

    private void Awake()
    {
        // 씬 어디서든 GameManager.Instance로 접근 가능하게 설정
        Instance = this;
    }

    private void Start()
    {
        // Phase 1 시작: 이동 잠금 (마우스 회전만 가능)
        playerController.canMove = false;
    }

    // ── Phase 2 진입 (CustomerManager에서 호출) ──
    public void OnPhase2Start()
    {
        IsPhase2 = true;            // Phase 2 진입 표시

        // 이동 해제
        playerController.canMove = true;

        // 미션 텍스트 등장
        missionManager.ShowMissions();

        Debug.Log("Phase 2 시작: 이동 가능 + 미션 등장");
    }
}
