using UnityEngine;

public class PlayerSpawnPointUpdater : MonoBehaviour
{
    private PlayerRespawner respawner;

    private void Start()
    {
        respawner = GetComponent<PlayerRespawner>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpawnPoint"))
        {
            Debug.Log("스폰 포인트 변경됨: " + other.transform.position);
            respawner.SetRespawnPoint(other.transform);
        }
    }
}