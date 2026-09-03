using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    private Vector2 currentSpawnPoint;

    private void Start()
    {
        if (currentSpawnPoint == Vector2.zero)
        {
            currentSpawnPoint = new Vector2(-6.44f, -1.73f); // 기본 위치
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void SetSpawnPoint(Vector2 position)
    {
        currentSpawnPoint = position;
        Debug.Log("Spawn point updated to: " + position);
    }

    public Vector2 GetSpawnPoint()
    {
        return currentSpawnPoint;
    }
}