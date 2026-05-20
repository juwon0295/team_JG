using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcController : MonoBehaviour
{
    public float speed = 2f;

    Vector3 startPos;
    Transform desk;

    List<Transform> activatedDeskChildren = new List<Transform>();

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

        GameObject deskObj = GameObject.Find("Desk");
        if (deskObj != null)
            desk = deskObj.transform;

        StartCoroutine(NpcRoutine());
    }

    IEnumerator NpcRoutine()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            yield return StartCoroutine(MoveTo(targets[i]));

            if (i == targets.Length - 1)
            {
                ActivateRandomDeskObjects();
                yield return new WaitForSeconds(2f);
                DeactivateDeskObjects();
            }
        }

        yield return StartCoroutine(MoveTo(startPos));
        Destroy(gameObject);
    }

    void ActivateRandomDeskObjects()
    {
        if (desk == null || desk.childCount == 0) return;

        activatedDeskChildren.Clear();

        List<Transform> children = new List<Transform>();
        for (int i = 0; i < desk.childCount; i++)
        {
            children.Add(desk.GetChild(i));
        }

        // 셔플
        for (int i = 0; i < children.Count; i++)
        {
            int rand = Random.Range(i, children.Count);
            Transform temp = children[i];
            children[i] = children[rand];
            children[rand] = temp;
        }

        // 1~4개 랜덤 선택 (자식 수 초과 방지)
        int count = Random.Range(1, Mathf.Min(5, children.Count + 1));

        for (int i = 0; i < count; i++)
        {
            children[i].gameObject.SetActive(true);
            activatedDeskChildren.Add(children[i]);
        }
    }

    void DeactivateDeskObjects()
    {
        foreach (Transform t in activatedDeskChildren)
        {
            if (t != null)
                t.gameObject.SetActive(false);
        }

        activatedDeskChildren.Clear();
    }

    IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            Vector3 dir = (target - transform.position).normalized;

            if (dir != Vector3.zero)
                transform.forward = dir;

            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );

            yield return null;
        }
    }
}
