using UnityEngine;
using System.Collections;

public class NpcController : MonoBehaviour
{
    public float speed = 2f;

    Vector3 target1 = new Vector3(0f, 0f, 0f);
    Vector3 target2 = new Vector3(10f, 0f, 0f);

    void Start()
    {
        StartCoroutine(NpcRoutine());
    }

    IEnumerator NpcRoutine()
    {
        // (0,0,0)으로 이동
        yield return StartCoroutine(MoveTo(target1));

        // 대사 출력
        Debug.Log("Hi");

        // 2초 대기
        yield return new WaitForSeconds(2f);

        // 다시 (10,0,0)으로 이동
        yield return StartCoroutine(MoveTo(target2));

        // 삭제
        Destroy(gameObject);
    }

    IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            Vector3 dir = (target - transform.position).normalized;

            // ✅ 바라보는 방향 설정
            if (dir != Vector3.zero)
            {
                transform.forward = dir;
            }

            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );

            yield return null;
        }
    }
}