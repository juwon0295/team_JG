using UnityEngine;
using TMPro;

public class PlayerScan : MonoBehaviour
{
    [Header("스캔 설정")]
    public float scanRange = 3f;            // 스캔 가능한 최대 거리
    public POSUIManager posManager;         // 포스기 연결

    [Header("UI")]
    public TextMeshProUGUI promptText;      // 조준 시 표시할 안내 텍스트

    void Update()
    {
        // 포스기 UI가 열려있으면 스캔 및 안내 텍스트 차단
        if (posManager.isOpen)
        {
            SetPrompt("");
            return;
        }

        // 매 프레임 조준 중인 물건 감지 → 안내 텍스트 갱신
        CheckAiming();

        // 좌클릭 시 스캔 시도
        if (Input.GetMouseButtonDown(0))
        {
            Scan();
        }
    }

    // ── 조준 중인 물건 감지 및 안내 텍스트 표시 ─────────────
    void CheckAiming()
    {
        // 화면 정중앙에서 Ray 발사
        Ray ray = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width / 2, Screen.height / 2)
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, scanRange))
        {
            ScanableObject item = hit.collider.GetComponent<ScanableObject>();

            if (item != null)
            {
                if (item.isScanned)
                {
                    // 이미 스캔된 물건 → 완료 안내 표시
                    SetPrompt("스캔 완료됨");
                }
                else
                {
                    // 아직 스캔 안 된 물건 → 가격 + 스캔 안내 표시
                    SetPrompt($"가격: {item.price}원\n[좌클릭] 스캔");
                }
                return;
            }
        }

        // 조준 중인 스캔 가능 물건이 없으면 텍스트 비우기
        SetPrompt("");
    }

    // ── 좌클릭 시 스캔 처리 ──────────────────────────────────
    void Scan()
    {
        // 화면 정중앙에서 Ray 발사
        Ray ray = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width / 2, Screen.height / 2)
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, scanRange))
        {
            ScanableObject item = hit.collider.GetComponent<ScanableObject>();

            // 스캔 가능한 물건이고 아직 스캔 안 됐으면 스캔 처리
            if (item != null && !item.isScanned)
            {
                item.isScanned = true;          // 스캔 완료 표시
                posManager.AddPrice(item.price); // 포스기에 가격 누적

                // 스캔 직후 안내 텍스트를 "스캔 완료됨"으로 즉시 갱신
                SetPrompt("스캔 완료됨");

                Debug.Log("스캔 성공: " + item.price);
            }
        }
    }

    // ── 안내 텍스트 설정 ─────────────────────────────────────
    void SetPrompt(string text)
    {
        if (promptText != null) promptText.text = text;
    }
}
