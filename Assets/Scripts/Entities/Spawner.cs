using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject skeletonPrefab;
    void Start()
    {
        StartCoroutine(SpawnAfterDelay(2f));
    }

    IEnumerator SpawnAfterDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            SpawnSkeleton();
        }
    }

    private int counter = 0;
    
    public void SpawnSkeleton()
    {
        if (skeletonPrefab != null)
        {
            counter += 1;
            if (counter >= 8)
            {
                return;
            }
            Instantiate(skeletonPrefab, transform.position, Quaternion.identity);
        }
    }
}
