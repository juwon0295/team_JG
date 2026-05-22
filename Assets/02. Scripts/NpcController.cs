using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcController : MonoBehaviour
{
    public float speed = 2f;
    public AudioSource footstepSound;


    Vector3 startPos;                                           // NPC 시작 위치 (루틴 끝나고 복귀용)
    Transform desk;                                             // 계산대 오브젝트 Transform

    List<Transform> activatedDeskChildren = new List<Transform>(); // 이번 손님이 올린 물건 목록

    // NPC가 이동할 경로 좌표 (순서대로 이동)
    Vector3[] targets = new Vector3[]
    {
        new Vector3(7.1f, 3.5f, 16.1f),
        new Vector3(7.3f, 3.5f, 22.8f),
        new Vector3(-1.2f, 3.5f, 23.3f),
        new Vector3(2f, 3.5f, 22.8f),
        new Vector3(2.19f, 3.5f, 18.56f),
        new Vector3(0f, 3.5f, 16.8f)     // 마지막 좌표 = 계산대 앞
    };

    void Start()
    {
        // 시작 위치 저장 (루틴 완료 후 복귀에 사용)
        startPos = transform.position;

        // 씬에서 "Desk" 오브젝트 찾아서 연결
        GameObject deskObj = GameObject.Find("Desk");
        if (deskObj != null)
            desk = deskObj.transform;

        // NPC 행동 루틴 시작
        StartCoroutine(NpcRoutine());
    }

    IEnumerator NpcRoutine()
    {
        // 경로 좌표를 순서대로 이동
        for (int i = 0; i < targets.Length; i++)
        {
            yield return StartCoroutine(MoveTo(targets[i]));

            // 마지막 좌표(계산대 앞) 도착 시
            if (i == targets.Length - 1)
            {
                // 계산대에 물건 랜덤 활성화
                ActivateRandomDeskObjects();

                // 결제 완료로 물건이 전부 비활성화될 때까지 대기
                yield return StartCoroutine(WaitUntilAllDeactivated());
            }
        }

        // 결제 완료 후 시작 위치로 복귀
        yield return StartCoroutine(MoveTo(startPos));

        // 복귀 완료 후 NPC 제거
        Destroy(gameObject);
    }

    void ActivateRandomDeskObjects()
    {
        if (desk == null || desk.childCount == 0) return;

        // 이전 목록 초기화
        activatedDeskChildren.Clear();

        // Desk의 자식 오브젝트를 리스트로 수집
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < desk.childCount; i++)
        {
            children.Add(desk.GetChild(i));
        }

        // Fisher-Yates 셔플로 랜덤 순서 섞기
        for (int i = 0; i < children.Count; i++)
        {
            int rand = Random.Range(i, children.Count);
            Transform temp = children[i];
            children[i] = children[rand];
            children[rand] = temp;
        }

        // 1~4개 랜덤 선택 (자식 수 초과 방지)
        int count = Random.Range(1, Mathf.Min(5, children.Count + 1));

        // 선택된 물건 활성화 + 목록에 추가
        for (int i = 0; i < count; i++)
        {
            children[i].gameObject.SetActive(true);
            activatedDeskChildren.Add(children[i]);
        }
    }

    // ── 결제 완료 후 이번 손님이 올린 물건 전부 비활성화 ──
    public void DeactivateDeskObjects()
    {
        foreach (Transform t in activatedDeskChildren)
        {
            if (t != null)
                t.gameObject.SetActive(false);
        }

        // 목록 초기화
        activatedDeskChildren.Clear();
    }

    IEnumerator MoveTo(Vector3 target)
    {
        // ✅ 이동 시작 → 발소리 재생
        if (footstepSound != null && !footstepSound.isPlaying)
            footstepSound.Play();


        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            Vector3 dir = (target - transform.position).normalized;

            // 이동 방향으로 NPC 회전
            if (dir != Vector3.zero)
                transform.forward = dir;

            // 목표 방향으로 한 프레임 이동
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );

            yield return null;
        }

        // ✅ 이동 끝 → 발소리 정지
        if (footstepSound != null && footstepSound.isPlaying)
            footstepSound.Stop();
    }

    IEnumerator WaitUntilAllDeactivated()
    {
        // 활성화된 물건이 하나라도 남아있으면 계속 대기
        while (true)
        {
            bool anyActive = false;

            foreach (Transform t in activatedDeskChildren)
            {
                // 활성화된 물건이 있으면 아직 결제 미완료
                if (t != null && t.gameObject.activeSelf)
                {
                    anyActive = true;
                    break;
                }
            }

            // 전부 비활성화됐으면 대기 종료 (결제 완료)
            if (!anyActive)
                yield break;

            yield return null;
        }
    }
}
