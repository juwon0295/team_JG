using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class POSUIManager : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject posPanel;            // 숫자판 전체 패널
    public TMP_Text displayText;           // 입력값 표시 텍스트

    private string currentInput = "";      // 현재 입력된 숫자
    private bool isOpen = false;           // UI 열림 여부

    void Start()
    {
        // 게임 시작 시 패널 숨기기
        posPanel.SetActive(false);
    }

    void Update()
    {
        // UI가 열려있을 때만 키보드 입력 처리
        if (!isOpen) return;

        // 숫자키 0~9 입력 감지 (일반 키 + 넘패드 동시 지원)
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown((KeyCode)(48 + i)) ||   // 일반 숫자키
                Input.GetKeyDown((KeyCode)(256 + i)))    // 넘패드 숫자키
            {
                AddNumber(i.ToString());
            }
        }

        // Backspace → 마지막 숫자 삭제
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DeleteLast();
        }

        // Enter 또는 넘패드 Enter → 창 닫기
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ClosePOS();
        }
    }

    // ── 포스기 열기 ──────────────────────────────
    public void OpenPOS()
    {
        isOpen = true;
        currentInput = "";
        UpdateDisplay();
        posPanel.SetActive(true);

        // 1인칭 게임: 마우스 커서 잠금 해제
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ── 포스기 닫기 ──────────────────────────────
    public void ClosePOS()
    {
        isOpen = false;
        posPanel.SetActive(false);

        // 1인칭 게임: 마우스 커서 다시 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ── 버튼/키보드에서 숫자 추가 ────────────────
    public void AddNumber(string number)
    {
        if (currentInput.Length >= 8) return;  // 최대 8자리 제한
        currentInput += number;
        UpdateDisplay();
    }

    // ── 마지막 숫자 하나 삭제 ─────────────────────
    public void DeleteLast()
    {
        if (currentInput.Length == 0) return;
        currentInput = currentInput.Substring(0, currentInput.Length - 1);
        UpdateDisplay();
    }

    // ── 전체 초기화 ───────────────────────────────
    public void ClearInput()
    {
        currentInput = "";
        UpdateDisplay();
    }

    // ── 디스플레이 텍스트 갱신 ───────────────────
    private void UpdateDisplay()
    {
        // 입력값 없으면 "0" 표시
        displayText.text = currentInput.Length > 0 ? currentInput : "0";
    }
}
