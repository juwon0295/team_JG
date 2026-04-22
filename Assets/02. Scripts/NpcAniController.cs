using UnityEngine;

public class NpcAniController : MonoBehaviour
{
    public Animator animator;

    Vector3 lastPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 delta = transform.position - lastPosition;

        // 로컬 기준으로 변환
        Vector3 localDelta = transform.InverseTransformDirection(delta);

        float v = (localDelta.z / Time.deltaTime) * 2f;
        float h = 0f;

        animator.SetFloat("v", v);
        animator.SetFloat("h", h);

        lastPosition = transform.position;
    }
}