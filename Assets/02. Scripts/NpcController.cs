using UnityEngine;
using System.Collections;

public class NpcController : MonoBehaviour
{
    public float speed = 2f;

    Vector3 startPos;

    Vector3[] targets = new Vector3[]
    {
        new Vector3(7.1f, 3.5f, 16.1f),
        new Vector3(7.3f, 3.5f, 22.8f),
        new Vector3(-1.2f, 3.5f, 23.3f),
        new Vector3(2f, 3.5f, 22.8f),
        new Vector3(2.19f, 3.5f, 18.56f),
        new Vector3(0f, 3.5f, 16.8f)
    };

    void Start()
    {
        startPos = transform.position;
        StartCoroutine(NpcRoutine());
    }

    IEnumerator NpcRoutine()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            yield return StartCoroutine(MoveTo(targets[i]));

            // 마지막 좌표 도착 시 실행
            if (i == targets.Length - 1)
            {
                Debug.Log("HI");
                yield return new WaitForSeconds(2f);
            }
        }

        yield return StartCoroutine(MoveTo(startPos));

        Destroy(gameObject);
    }

    IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            Vector3 dir = (target - transform.position).normalized;

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
