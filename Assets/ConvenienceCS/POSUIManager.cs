using UnityEngine;
using TMPro;

public class POSManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI totalPriceText; // 총 가격 표시 텍스트
    public TextMeshProUGUI inputText;      // 플레이어 입력값 표시 텍스트
    public GameObject errorText;           // "가격이 다릅니다" 메시지 오브젝트

    private int totalPrice = 0;      // 현재까지 스캔된 총 가격
    private string currentInput = ""; // 플레이어가 입력한 숫자
    private bool isActive = false;    // 포스기 활성화 상태 (켜짐/꺼짐)

    void Update()
    {
        // 포스기가 꺼져 있으면 입력 받지 않음
        if (!isActive) return;

        HandleInput();
    }

    // 물건 스캔 시 호출 → 가격 누적
    public void AddPrice(int price)
    {
        totalPrice += price; // 총 가격에 추가
        UpdateUI();          // UI 갱신
    }

    // 포스기 켜기
    public void OpenPOS()
    {
        isActive = true;       // 입력 가능 상태로 변경
        currentInput = "";     // 입력값 초기화

        if (errorText != null)
            errorText.SetActive(false); // 에러 메시지 숨김

        UpdateUI();
    }

    // 키보드 입력 처리
    void HandleInput()
    {
        // 숫자 키 입력 (0~9)
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                currentInput += i.ToString(); // 문자열에 숫자 추가
                UpdateUI();
            }
        }

        // Enter 키 → 가격 비교
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CheckPrice();
        }

        // Backspace → 한 자리 삭제
        if (Input.GetKeyDown(KeyCode.Backspace) && currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            UpdateUI();
        }
    }

    // 입력값과 총 가격 비교
    void CheckPrice()
    {
        // 입력값이 비어있으면 0으로 처리
        int inputPrice = int.Parse(currentInput == "" ? "0" : currentInput);

        if (inputPrice == totalPrice)
        {
            // 결제 성공
            Debug.Log("결제 완료");

            totalPrice = 0;   // 총 가격 초기화
            currentInput = ""; // 입력값 초기화
            isActive = false; // 포스기 종료

            UpdateUI();
        }
        else
        {
            // 가격 틀림
            Debug.Log("가격 불일치");

            if (errorText != null)
                errorText.SetActive(true); // 에러 메시지 표시
        }
    }

    // UI 텍스트 갱신
    void UpdateUI()
    {
        if (totalPriceText != null)
            totalPriceText.text = "총 가격: " + totalPrice.ToString();

        if (inputText != null)
            inputText.text = currentInput;
    }
}
