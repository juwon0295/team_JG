using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkSpeed = 4f;      // 걷는 속도
    [SerializeField] float runSpeed = 7f;       // 달리기 속도
    [SerializeField] float gravity = -9.81f;    // 중력 값
    [SerializeField] float jumpHeight = 2f;     // 점프 높이
    [SerializeField] float mouseSpeed = 1.5f;   // 화면 회전 감도

    [Header("UI 연결")]
    public POSUIManager posUI;                  // 포스기 UI 열림 여부 확인용

    float xRot;             // 카메라 상하 회전값 저장
    Vector3 velo;           // 중력 및 점프 방향 저장
    Transform camTr;        // 카메라 트랜스폼
    CharacterController cc; // 캐릭터 컨트롤러 컴포넌트

    void Start()
    {
        // 캐릭터 컨트롤러 컴포넌트 할당
        cc = GetComponent<CharacterController>();

        // 마우스 커서 잠금 (화면에 안 보이게)
        Cursor.lockState = CursorLockMode.Locked;

        // 메인 카메라 트랜스폼 저장
        camTr = Camera.main.transform;
    }

    void Update()
    {
        MoveAndJump();
        Look();
    }

    void MoveAndJump()
    {
        // 수평(A/D), 수직(W/S) 입력 받기
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 땅에 닿아있으면 낙하 속도 초기화
        bool grounded = cc.isGrounded;
        if (grounded && velo.y < 0) velo.y = -2f;

        // Shift 누르면 달리기, 아니면 걷기
        float curSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // 입력 방향으로 이동 벡터 계산
        Vector3 movDir = transform.right * h + transform.forward * v;
        cc.Move(movDir * curSpeed * Time.deltaTime);

        // 점프 입력 감지 (땅에 있을 때만 점프 가능)
        if (Input.GetButtonDown("Jump") && grounded)
            velo.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // 중력 적용
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

        // 마우스 X, Y 입력 받기
        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed;

        // 상하 회전값 계산 및 -90 ~ 90도로 제한
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        // 카메라 상하 회전 적용
        camTr.localRotation = Quaternion.Euler(xRot, 0f, 0f);

        // 플레이어 몸체 좌우 회전 적용
        transform.Rotate(Vector3.up * mouseX);
    }
}
