using UnityEngine;
using TMPro;

public class StoreMissionManager : MonoBehaviour
{
    public static StoreMissionManager Instance; // 싱글톤

    [Header("미션 패널 (왼쪽 위)")]
    public GameObject missionPanel;         // 미션 전체 패널

    [Header("미션 텍스트")]
    public TMP_Text missionRestock;         // "물건을 채우세요"
    public TMP_Text missionTrash;           // "쓰레기를 치우세요"

    private bool restockDone = false;       // 물건 채우기 완료 여부
    private bool trashDone = false;         // 쓰레기 치우기 완료 여부

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 게임 시작 시 미션 패널 숨기기
        missionPanel.SetActive(false);
    }

    // ── Phase 2 진입 시 GameManager에서 호출 ─────
    public void ShowMissions()
    {
        missionPanel.SetActive(true);

        // 텍스트 초기 상태 (줄긋기 없음, 완전 불투명)
        SetMissionText(missionRestock, false);
        SetMissionText(missionTrash, false);
    }

    // ── 물건 채우기 완료 시 PlayerPickup에서 호출 ──
    public void CompleteRestock()
    {
        if (restockDone) return;    // 이미 완료됐으면 무시
        restockDone = true;
        SetMissionText(missionRestock, true);
        Debug.Log("미션 완료: 물건 채우기");
    }

    // ── 쓰레기 치우기 완료 시 TrashPickupSystem에서 호출 ──
    public void CompleteTrash()
    {
        if (trashDone) return;      // 이미 완료됐으면 무시
        trashDone = true;
        SetMissionText(missionTrash, true);
        Debug.Log("미션 완료: 쓰레기 치우기");
    }

    // ── 미션 텍스트 스타일 변경 ───────────────────
    private void SetMissionText(TMP_Text text, bool isCompleted)
    {
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
            // 초기 상태: 줄긋기 없음 + 완전 불투명
            text.fontStyle = FontStyles.Normal;
            Color c = text.color;
            c.a = 1f;
            text.color = c;
        }
    }
}
