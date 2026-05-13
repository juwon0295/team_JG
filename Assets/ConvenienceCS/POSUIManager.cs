using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class POSUIManager : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject posPanel;             // 숫자판 전체 패널
    public TMP_Text displayText;            // 입력값 표시 텍스트
    public TMP_Text totalPriceText;         // 스캔된 총 가격 표시 텍스트
    public GameObject errorText;            // 가격 불일치 메시지 오브젝트

    private string currentInput = "";       // 현재 입력된 숫자
    private int totalPrice = 0;             // 스캔으로 누적된 총 가격
    public bool isOpen = false;             // UI 열림 여부

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

        // Enter 또는 넘패드 Enter → 가격 비교
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            CheckPrice();
        }
    }

    // ── 포스기 열기 ──────────────────────────────
    public void OpenPOS()
    {
        isOpen = true;
        currentInput = "";

        // 에러 메시지 숨기기
        if (errorText != null)
            errorText.SetActive(false);

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

    // ── 스캔 시 가격 누적 (PlayerScan에서 호출) ──
    public void AddPrice(int price)
    {
        totalPrice += price;        // 스캔된 가격 누적
        UpdateDisplay();
    }

    // ── 버튼/키보드에서 숫자 추가 ────────────────
    public void AddNumber(string number)
    {
        // 입력값이 8자리 넘지 않게 제한
        if (currentInput.Length + number.Length > 8) return;

        // 맨 앞에 0이 여러 개 오는 것 방지 (예: 000123 방지)
        if (currentInput == "" && number == "00") return;
        if (currentInput == "" && number == "000") return;

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

    // ── 입력값과 총 가격 비교 ─────────────────────
    public void CheckPrice()
    {
        // 입력값이 비어있으면 0으로 처리
        int inputPrice = currentInput == "" ? 0 : int.Parse(currentInput);

        if (inputPrice == totalPrice)
        {
            // 결제 성공 → 초기화 후 포스기 닫기
            Debug.Log("결제 완료");
            totalPrice = 0;
            currentInput = "";
            ClosePOS();
        }
        else
        {
            // 가격 불일치 → 에러 메시지 표시
            Debug.Log("가격 불일치");
            if (errorText != null)
                errorText.SetActive(true);
        }
    }

    // ── 디스플레이 텍스트 갱신 ───────────────────
    private void UpdateDisplay()
    {
        // 입력값 없으면 "0" 표시
        displayText.text = currentInput.Length > 0 ? currentInput : "0";

        // 총 가격 텍스트 갱신
        if (totalPriceText != null)
            totalPriceText.text = "총 가격: " + totalPrice.ToString() + "원";
    }
}
