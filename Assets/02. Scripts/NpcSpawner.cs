using UnityEngine;
using System.Collections;

public class NpcSpawner : MonoBehaviour
{
    public GameObject[] npcPrefabs; // 여러 개 넣기

    GameObject currentNpc;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float delay = Random.Range(3f, 10f);
            yield return new WaitForSeconds(delay);

<<<<<<< HEAD
            // 랜덤 NPC 선택
            GameObject prefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];

            currentNpc = Instantiate(
                prefab,
                new Vector3(1f, 3.5f, 15f),
                Quaternion.identity
            );

            // NPC 사라질 때까지 대기
            while (currentNpc != null)
            {
                yield return null;
            }
=======
            Instantiate(npcPrefab, new Vector3(10f, 0f, 0f), Quaternion.identity);
>>>>>>> 1b352ea8cb26a08390a7b98ab55b2bd03fb92bad
        }
    }
}
