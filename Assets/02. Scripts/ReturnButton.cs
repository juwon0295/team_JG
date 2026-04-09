using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ReturnButton : MonoBehaviour
{
    public float rayDistance = 1.5f; // 레이가 닿는 최대 거리
    public TextMeshProUGUI interactText; // 안내 텍스트 연결할 변수

    private Camera cam;

    void Start()
    {
        cam = Camera.main; // 메인 카메라 가져오기
        interactText.gameObject.SetActive(false); // 시작할 때 텍스트 숨기기
    }

    void Update()
    {
        // 화면 정중앙에서 레이(Ray) 발사
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance)) // 레이에 뭔가 맞았을 때
        {
            if (hit.collider.gameObject == this.gameObject) // 맞은 게 이 버튼일 때
            {
                interactText.gameObject.SetActive(true); // 텍스트 표시

                if (Input.GetKeyDown(KeyCode.E)) // E키를 눌러서 씬 이동
                {
                    SceneManager.LoadScene("MainMenuScene");
                }
            }
            else
            {
                interactText.gameObject.SetActive(false); // 다른 오브젝트를 보고 있으면 텍스트 숨기기
            }
        }
        else
        {
            interactText.gameObject.SetActive(false); // 아무것도 안 맞으면 텍스트 숨기기
        }
    }
}