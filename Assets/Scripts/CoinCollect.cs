using UnityEngine;

public class CoinCollect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // CoinManager에 카운트 추가
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoin();
            }

            // 코인 제거
            Destroy(gameObject);
        }
    }
}