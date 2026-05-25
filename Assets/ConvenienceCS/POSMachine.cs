using UnityEngine;
using TMPro;

public class POSMachine : MonoBehaviour
{
    [Header("상호작용 설정")]
    public float interactRange = 3f;        // 조준점 기준 상호작용 가능 거리
    public POSUIManager posUI;              // POSUIManager 연결

    [Header("UI")]
    public TextMeshProUGUI promptText;      // 포스기 조준 시 안내 텍스트

    private Camera playerCamera;           // 플레이어 카메라

    void Start()
    {
        // 메인 카메라를 플레이어 카메라로 사용
        playerCamera = Camera.main;
    }

    void Update()
    {
        // UI가 이미 열려있으면 안내 텍스트 숨기고 E키 입력 무시
        if (posUI.isOpen)
        {
            SetPrompt("");
            return;
        }

        // 화면 정중앙(조준점)에서 앞쪽으로 Ray 발사
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            // Ray에 맞은 오브젝트가 이 포스기인지 확인
            if (hit.transform == this.transform)
            {
                // 스캔 안 된 물건이 남아있으면 안내 텍스트 다르게 표시
                if (!AllItemsScanned())
                {
                    SetPrompt("스캔되지 않은 물건이 있습니다");
                }
                else
                {
                    // 정상적으로 열 수 있는 상태
                    SetPrompt("[E] 포스기 열기");
                }

                // E키를 누르면 포스기 UI 열기 시도
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (!AllItemsScanned())
                    {
                        Debug.Log("아직 스캔 안 된 물건이 있습니다.");
                        return;
                    }

                    posUI.OpenPOS();
                }

                return;
            }
        }

        // 포스기를 조준하지 않으면 텍스트 비우기
        SetPrompt("");
    }

    // ── 계산대 위 활성화된 물건이 전부 스캔됐는지 확인 ──────
    bool AllItemsScanned()
    {
        // "Desk" 오브젝트의 자식 중 활성화된 것만 확인
        GameObject deskObj = GameObject.Find("Desk");
        if (deskObj == null) return true;

        foreach (Transform child in deskObj.transform)
        {
            // 활성화된 물건 중 스캔 안 된 것이 있으면 false
            if (child.gameObject.activeSelf)
            {
                ScanableObject scanable = child.GetComponent<ScanableObject>();
                if (scanable != null && !scanable.isScanned)
                    return false;
            }
        }

        return true;
    }

    // ── 안내 텍스트 설정 ─────────────────────────────────────
    void SetPrompt(string text)
    {
        if (promptText != null) promptText.text = text;
    }
}
