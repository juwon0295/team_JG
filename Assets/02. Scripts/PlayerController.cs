using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkSpeed = 4f; //걷는 속도
    [SerializeField] float runSpeed = 7f; //달리기 속도

    [SerializeField] float gravity = -9.81f; // 중력 값
    [SerializeField] float jumpHeight = 2f; // 점프 높이

    [SerializeField] float mouseSpeed = 1.5f; //화면 회전 감도

    float xRot; //x의 회전값 저장
    Vector3 velo; //이동 방향 저장
    Transform camTr; //카메라 트랜스폼

    CharacterController cc; //캐릭터컨트롤러 컴포넌트

    void Start() //cc에 캐릭터 컨트롤러 컴포넌트 할당
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; //마우스커서 잠금 (화면에 안보이게 지정)

        camTr = Camera.main.transform;
    }

    void Update()
    {
        MoveAndJump();
        Look();
    }

    void MoveAndJump()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool grounded = cc.isGrounded;
        if (grounded && velo.y < 0) velo.y = -2f;

        float curSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        Vector3 movDir = transform.right * h + transform.forward * v;

        cc.Move(movDir * curSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && grounded) velo.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        velo.y += gravity * Time.deltaTime;

        cc.Move(velo * Time.deltaTime);
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed; //마우스의 x축입력 저장
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed; //마우스의 y축입력 저장

        xRot -= mouseY; //상하 회전 값 계산
        xRot = Mathf.Clamp(xRot, -90f, 90f); //각도 제한 90도

        camTr.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}