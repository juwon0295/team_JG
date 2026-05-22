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

    [Header("플레이어 연결")]
    public PlayerController playerController;   // 플레이어 컨트롤러
    public CharacterController controller;      // 캐릭터 컨트롤러

    private string currentInput = "";       // 현재 입력된 숫자
    private int totalPrice = 0;             // 스캔으로 누적된 총 가격
    public bool isOpen = false;             // UI 열림 여부

    void Start()
    {
        // 게임 시작 시 패널 숨기기
        posPanel.SetActive(false);
        isOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // UI가 열려있을 때만 키보드 입력 처리
        if (!isOpen) return;

        // ESC → 포스기 닫기
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            currentInput = "";  // 입력값 초기화
            ClosePOS();
            return;
        }

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
            DeleteLast();

        // Enter 또는 넘패드 Enter → 가격 비교
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            CheckPrice();
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

        // 마우스 커서 잠금 해제
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 이동 차단 (Look은 PlayerController.Look()의 isOpen 체크로 자동 차단)
        if (controller != null) controller.enabled = false;
        if (playerController != null) playerController.canMove = false;
    }

    // ── 포스기 닫기 ──────────────────────────────
    public void ClosePOS()
    {
        isOpen = false;
        posPanel.SetActive(false);

        // 마우스 커서 다시 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 이동 재개 (Phase 2에서만 실제로 움직임 - canMove는 GameManager가 관리)
        if (controller != null) controller.enabled = true;
        if (playerController != null && GameManager.Instance != null)
        {
            // GameManager의 Phase 상태에 따라 이동 여부 결정
            // Phase 2라면 canMove = true, Phase 1이라면 false 유지
            playerController.canMove = GameManager.Instance.IsPhase2;
        }
    }

    // ── 스캔 시 가격 누적 (PlayerScan에서 호출) ──
    public void AddPrice(int price)
    {
        totalPrice += price;
        UpdateDisplay();
    }

    // ── 버튼/키보드에서 숫자 추가 ────────────────
    public void AddNumber(string number)
    {
        // 입력값이 8자리 넘지 않게 제한
        if (currentInput.Length + number.Length > 8) return;

        // 맨 앞에 0이 여러 개 오는 것 방지
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
        int inputPrice = currentInput == "" ? 0 : int.Parse(currentInput);

        // 총 가격이 0원이면 결제 차단 (스캔된 물건 없음)
        if (totalPrice == 0)
        {
            Debug.Log("스캔된 물건이 없습니다.");
            if (errorText != null)
                errorText.SetActive(true);
            return;
        }

        if (inputPrice == totalPrice)
        {
            // 결제 성공 → 계산대 물건 비활성화
            Debug.Log("결제 완료");

            // 계산대 물건 스캔 상태 초기화 ← 추가
            GameObject deskObj = GameObject.Find("Desk");
            if (deskObj != null)
            {
                foreach (Transform child in deskObj.transform)
                {
                    ScanableObject scanable = child.GetComponent<ScanableObject>();
                    if (scanable != null)
                        scanable.isScanned = false;
                }
            }

            // 현재 씬의 NpcController를 찾아서 계산대 물건 비활성화
            NpcController npc = FindAnyObjectByType<NpcController>();
            if (npc != null)  // ← 추가
                npc.DeactivateDeskObjects();  // ← 추가

            CustomerManager.Instance.OnCustomerServed();
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
        displayText.text = currentInput.Length > 0 ? currentInput : "0";

        if (totalPriceText != null)
            totalPriceText.text = "총 가격: " + totalPrice.ToString() + "원";
    }
}
