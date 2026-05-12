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

            Instantiate(npcPrefab, new Vector3(1f, 4.65f, 15f), Quaternion.identity);
        }
    }
}