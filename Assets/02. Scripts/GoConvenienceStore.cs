using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GoConvenienceStoreButton : MonoBehaviour
{
    public float interactDistance = 3f; // 상호작용 가능한 거리
    public TextMeshProUGUI interactText; // 안내 텍스트 연결할 변수
    private Transform player;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        interactText.gameObject.SetActive(false); // 시작할 때 텍스트 숨기기
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance < interactDistance)
        {
            interactText.gameObject.SetActive(true); // 가까이 가면 텍스트 표시

            if (Input.GetKeyDown(KeyCode.E)) // E키를 눌러서 씬 이동
            {
                SceneManager.LoadScene("ConvenienceStoreScene");
            }
        }
        else
        {
            interactText.gameObject.SetActive(false); // 멀어지면 텍스트 숨기기
        }
    }
}