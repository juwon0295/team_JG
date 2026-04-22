using UnityEngine;
using UnityEngine.SceneManagement;

public class EscMenuController : MonoBehaviour
{
    public GameObject escMenuPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isActive = escMenuPanel.activeSelf;
            escMenuPanel.SetActive(!isActive);

            if (!isActive)
            {
                // 패널 열릴 때 → 마우스 보이게 + 잠금 해제
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                // 패널 닫힐 때 → 마우스 다시 잠금
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void GoToMainScene()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
