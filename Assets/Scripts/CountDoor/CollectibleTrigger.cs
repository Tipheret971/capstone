using UnityEngine;

public class CollectibleTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ProgressCounter.IncreaseCount();
            Destroy(gameObject); // 이 오브젝트는 한 번 수집되면 사라진다고 가정
        }
    }
}