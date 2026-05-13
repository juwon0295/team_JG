using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkSpeed = 1f;
    [SerializeField] float runSpeed = 2f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float mouseSpeed = 1.5f;

    [Header("UI 연결")]
    public POSUIManager posUI;

    float xRot;
    Vector3 velo;
    Transform camTr;
    CharacterController cc;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        camTr = Camera.main.transform;
    }

    void Update()
    {
        Move();
        Look();
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool grounded = cc.isGrounded;
        if (grounded && velo.y < 0) velo.y = -2f;

        float curSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        Vector3 movDir = transform.right * h + transform.forward * v;
        cc.Move(movDir * curSpeed * Time.deltaTime);

        // 점프 제거, 중력만 유지
        velo.y += gravity * Time.deltaTime;
        cc.Move(velo * Time.deltaTime);
    }

    void Look()
    {

        // 포스기 UI가 열려있으면 카메라 회전 차단
        if (posUI != null && posUI.isOpen) return;
        // 나중에 다른 UI 추가 시 아래처럼 추가하면 됨
        // if (inventoryUI.isOpen) return;
        // if (dialogueUI.isOpen) return;
=======
        
 75ccd79d10acabed1a0441ff46b60dd9ea24c33a

        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        camTr.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
