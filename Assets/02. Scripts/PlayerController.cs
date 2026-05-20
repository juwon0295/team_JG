using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkSpeed = 1f;      // 걷기 속도
    [SerializeField] float runSpeed = 2f;       // 달리기 속도 (Shift 누를 때)
    [SerializeField] float gravity = -9.81f;    // 중력 값
    [SerializeField] float mouseSpeed = 1.5f;   // 마우스 감도

    [Header("UI 연결")]
    public POSUIManager posUI;                  // 포스기 UI (열려있으면 카메라 회전 차단)

    [Header("이동 잠금 (GameManager에서 제어)")]
    public bool canMove = true;                 // false면 이동 불가 (Phase 1에서 GameManager가 false로 설정)

    float xRot;         // 카메라 상하 회전값 누적
    Vector3 velo;       // 중력 계산용 수직 속도
    Transform camTr;    // 메인 카메라 Transform
    CharacterController cc; // 캐릭터 컨트롤러

    void Start()
    {
        // 컴포넌트 초기화
        cc = GetComponent<CharacterController>();

        // 게임 시작 시 마우스 커서 숨기기 및 화면 중앙 고정
        Cursor.lockState = CursorLockMode.Locked;

        // 메인 카메라 Transform 캐싱
        camTr = Camera.main.transform;
    }

    void Update()
    {
        Move();
        Look();
    }

    void Move()
    {
        // Phase 1: GameManager가 canMove = false로 설정하면 이동 불가
        if (!canMove) return;

        // 수평(A/D), 수직(W/S) 입력값 받기
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 땅에 닿아 있을 때 수직 속도를 살짝 아래로 유지 (바닥 감지 안정화)
        bool grounded = cc.isGrounded;
        if (grounded && velo.y < 0) velo.y = -2f;

        // Shift 누르면 달리기, 아니면 걷기
        float curSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // 이동 방향 계산 (좌우 + 앞뒤)
        Vector3 movDir = transform.right * h + transform.forward * v;
        cc.Move(movDir * curSpeed * Time.deltaTime);

        // 중력 적용 (점프 없음)
        velo.y += gravity * Time.deltaTime;
        cc.Move(velo * Time.deltaTime);
    }

    void Look()
    {
        // 포스기 UI가 열려있으면 카메라 회전 차단 (마우스가 UI 조작용으로 풀려있기 때문)
        if (posUI != null && posUI.isOpen) return;

        // 마우스 이동량 받기
        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed; // 좌우 회전
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed; // 상하 회전

        // 상하 회전값 누적 후 -90 ~ 90도로 제한 (뒤집힘 방지)
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        // 카메라는 상하 회전, 플레이어 본체는 좌우 회전
        camTr.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
