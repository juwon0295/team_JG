using UnityEngine;
using TMPro;

public class StoreMissionManager : MonoBehaviour
{
    public static StoreMissionManager Instance;

    [Header("미션 패널 (왼쪽 위)")]
    public GameObject missionPanel;

    [Header("Phase 1 미션 텍스트")]
    public TMP_Text missionCustomer;            // "손님을 받으세요 (0/2)"

    [Header("Phase 2 미션 텍스트")]
    public TMP_Text missionCustomerPhase2;      // "손님을 계속 받으세요"
    public TMP_Text missionRestock;             // "물건을 채우세요"
    public TMP_Text missionTrash;               // "쓰레기를 치우세요"

    private bool restockDone = false;           // 물건 채우기 완료 여부
    private bool trashDone = false;             // 쓰레기 치우기 완료 여부

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 게임 시작 시 패널 표시 + Phase 1 미션만 보이게
        missionPanel.SetActive(true);

        // Phase 1 텍스트 초기화
        UpdateCustomerMission(0, 2);

        // Phase 2 텍스트는 숨기기
        if (missionCustomerPhase2 != null) missionCustomerPhase2.gameObject.SetActive(false);
        if (missionRestock != null) missionRestock.gameObject.SetActive(false);
        if (missionTrash != null) missionTrash.gameObject.SetActive(false);
    }

    // ── 손님 카운트 갱신 (CustomerManager에서 호출) ──
    public void UpdateCustomerMission(int current, int required)
    {
        if (missionCustomer != null)
            missionCustomer.text = $"손님을 받으세요 ({current}/{required})";
    }

    // ── Phase 2 진입 시 GameManager에서 호출 ─────
    public void ShowMissions()
    {
        // Phase 1 텍스트 완료 처리
        SetMissionText(missionCustomer, true);

        // Phase 2 텍스트 등장
        if (missionCustomerPhase2 != null) missionCustomerPhase2.gameObject.SetActive(true);
        if (missionRestock != null) missionRestock.gameObject.SetActive(true);
        if (missionTrash != null) missionTrash.gameObject.SetActive(true);

        SetMissionText(missionCustomerPhase2, false);
        SetMissionText(missionRestock, false);
        SetMissionText(missionTrash, false);
    }

    // ── 물건 채우기 완료 시 PlayerPickup에서 호출 ──
    public void CompleteRestock()
    {
        if (restockDone) return;
        restockDone = true;
        SetMissionText(missionRestock, true);
        Debug.Log("미션 완료: 물건 채우기");
    }

    // ── 쓰레기 치우기 완료 시 TrashPickupSystem에서 호출 ──
    public void CompleteTrash()
    {
        if (trashDone) return;
        trashDone = true;
        SetMissionText(missionTrash, true);
        Debug.Log("미션 완료: 쓰레기 치우기");
    }

    // ── 미션 텍스트 스타일 변경 ───────────────────
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
