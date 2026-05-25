using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StoreMissionManager : MonoBehaviour
{
    public static StoreMissionManager Instance;

    [Header("미션 패널")]
    public GameObject missionPanel;             // 전체 미션 패널

    [Header("Phase 1 미션 텍스트")]
    public TMP_Text missionCustomer;            // "손님을 받으세요 (0/2)"

    [Header("Phase 2 미션 텍스트")]
    public TMP_Text missionCustomerPhase2;      // "손님을 계속 받으세요"
    public TMP_Text missionRestock;             // "물건을 채우세요"
    public TMP_Text missionTrash;               // "쓰레기를 치우세요"

    [Header("완료 패널")]
    public GameObject completePanel;            // 모든 미션 완료 시 표시할 패널

    private bool restockDone = false;
    private bool trashDone = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        missionPanel.SetActive(true);

        // 완료 패널은 시작 시 숨기기
        if (completePanel != null)
            completePanel.SetActive(false);

        // Phase 1: 손님 미션만 표시
        if (missionCustomer != null) missionCustomer.gameObject.SetActive(true);

        // Phase 2 텍스트는 전부 숨기기
        if (missionCustomerPhase2 != null) missionCustomerPhase2.gameObject.SetActive(false);
        if (missionRestock != null) missionRestock.gameObject.SetActive(false);
        if (missionTrash != null) missionTrash.gameObject.SetActive(false);

        // Phase 1 텍스트 초기화
        UpdateCustomerMission(0, 2);
    }

    void Update()
    {
        // ── 임시 테스트용: F2 누르면 강제로 모든 미션 완료 ──
        // 테스트 끝나면 이 Update() 전체 삭제
        if (Input.GetKeyDown(KeyCode.F2))
        {
            CompleteRestock();
            CompleteTrash();
        }
    }

    // ── 손님 카운트 갱신 (CustomerManager에서 호출) ──────────
    public void UpdateCustomerMission(int current, int required)
    {
        if (missionCustomer != null)
            missionCustomer.text = $"손님을 받으세요 ({current}/{required})";
    }

    // ── Phase 2 진입 시 GameManager에서 호출 ─────────────────
    public void ShowMissions()
    {
        // Phase 1 텍스트 숨기기
        if (missionCustomer != null) missionCustomer.gameObject.SetActive(false);

        // Phase 2 텍스트 표시
        if (missionCustomerPhase2 != null) missionCustomerPhase2.gameObject.SetActive(true);
        if (missionRestock != null) missionRestock.gameObject.SetActive(true);
        if (missionTrash != null) missionTrash.gameObject.SetActive(true);

        SetMissionText(missionCustomerPhase2, false);
        SetMissionText(missionRestock, false);
        SetMissionText(missionTrash, false);
    }

    // ── 물건 채우기 완료 시 PlayerPickup에서 호출 ────────────
    public void CompleteRestock()
    {
        if (restockDone) return;
        restockDone = true;
        SetMissionText(missionRestock, true);
        Debug.Log("미션 완료: 물건 채우기");

        // 두 미션 모두 완료됐는지 확인
        CheckAllMissionsDone();
    }

    // ── 쓰레기 치우기 완료 시 TrashPickupSystem에서 호출 ─────
    public void CompleteTrash()
    {
        if (trashDone) return;
        trashDone = true;
        SetMissionText(missionTrash, true);
        Debug.Log("미션 완료: 쓰레기 치우기");

        // 두 미션 모두 완료됐는지 확인
        CheckAllMissionsDone();
    }

    // ── 모든 미션 완료 여부 확인 ─────────────────────────────
    private void CheckAllMissionsDone()
    {
        // 물건 채우기 + 쓰레기 치우기 둘 다 완료됐을 때
        if (restockDone && trashDone)
        {
            // 미션 패널 숨기기
            if (missionPanel != null)
                missionPanel.SetActive(false);

            // 완료 패널 표시
            if (completePanel != null)
                completePanel.SetActive(true);

            // 마우스 커서 잠금 해제 (버튼 클릭 가능하도록)
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log("모든 미션 완료!");
        }
    }

    // ── 메인 메뉴로 돌아가기 버튼에서 호출 ──────────────────
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    // ── 미션 텍스트 스타일 변경 ───────────────────────────────
    private void SetMissionText(TMP_Text text, bool isCompleted)
    {
        if (text == null) return;

        if (isCompleted)
        {
            // 줄긋기 + 반투명 (40%)
            text.fontStyle = FontStyles.Strikethrough;
            Color c = text.color;
            c.a = 0.4f;
            text.color = c;
        }
        else
        {
            // 초기 상태
            text.fontStyle = FontStyles.Normal;
            Color c = text.color;
            c.a = 1f;
            text.color = c;
        }
    }
}
