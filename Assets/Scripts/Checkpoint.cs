using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnManager.Instance.SetSpawnPoint(transform.position); // 현재 위치를 스폰 위치로 저장
        }
    }
}