using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GoConvenienceStoreButton : MonoBehaviour
{
    [Header("상호작용 설정")]
    public float interactRange = 2f;            // 상호작용 가능한 최대 거리
    public TextMeshProUGUI interactText;         // 안내 텍스트

    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        // 시작 시 안내 텍스트 숨기기
        if (interactText != null)
            interactText.gameObject.SetActive(false);
    }

    void Update()
    {
        // 화면 정중앙에서 Ray 발사
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            // 조준한 오브젝트가 이 버튼인지 확인
            if (hit.transform == this.transform)
            {
                // 안내 텍스트 표시
                if (interactText != null)
                {
                    interactText.gameObject.SetActive(true);
                    interactText.text = "[E] 편의점 알바 시작";
                }

                // E키 누르면 씬 이동
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SceneManager.LoadScene("ConvenienceStoreScene");
                }

                return;
            }
        }

        // 버튼을 조준하지 않으면 텍스트 숨기기
        if (interactText != null)
            interactText.gameObject.SetActive(false);
    }
}
