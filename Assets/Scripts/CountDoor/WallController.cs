using UnityEngine;
using System.Collections;

public class WallController : MonoBehaviour
{
    public GameObject wallToOpen;           // 실제 열리는 벽 오브젝트
    public int requiredCount = 3;           // 열기 위해 필요한 수치
    private bool isOpening = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpening) return;

        if (other.CompareTag("Player"))
        {
            if (ProgressCounter.collectedCount >= requiredCount)
            {
                Debug.Log("버튼 작동! 2초 후 벽이 열립니다.");
                isOpening = true;
                StartCoroutine(OpenWallAfterDelay());
            }
            else
            {
                Debug.Log("조건 부족: " + ProgressCounter.collectedCount);
            }
        }
    }

    private IEnumerator OpenWallAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        if (wallToOpen != null)
        {
            Debug.Log("벽 열림!");
            Destroy(wallToOpen);
        }
        else
        {
            Debug.LogWarning("wallToOpen이 할당되지 않았습니다.");
        }
    }
}