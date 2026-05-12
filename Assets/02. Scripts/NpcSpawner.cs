using UnityEngine;
using System.Collections;

public class NpcSpawner : MonoBehaviour
{
    public GameObject npcPrefab;

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

            Instantiate(npcPrefab, new Vector3(10f, 0f, 0f), Quaternion.identity);
        }
    }
}