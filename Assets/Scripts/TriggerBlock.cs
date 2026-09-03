using UnityEngine;

public class TriggerBlock : MonoBehaviour
{
    public GameObject[] targetGroundsDisapp; // 사라질 땅 오브젝트들
    public GameObject[] targetGroundsApp; // 나타날 발판들

    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            foreach (GameObject ground in targetGroundsDisapp)
            {
                ground.SetActive(false); // 땅을 비활성화 (사라지게 함)
            }

            foreach (GameObject platform in targetGroundsApp)
            {
                platform.SetActive(true); // 발판 나타남
            }

            //트리거 자신도 사라지게
            gameObject.SetActive(false);
        }
    }
}