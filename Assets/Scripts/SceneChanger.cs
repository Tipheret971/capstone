using UnityEngine;                      // 유니티 엔진 기능 사용
using UnityEngine.SceneManagement;     // 씬 전환 기능 사용
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    private bool isTriggered = false;  // 씬 전환 중복 방지용 플래그

    // 충돌이 발생했을 때 자동으로 호출되는 함수 (2D 충돌 전용)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트의 태그가 "Player"이고, 아직 씬 전환이 안 되었을 때
        if (!isTriggered && collision.gameObject.CompareTag("Player"))
        {
            // 이 스크립트가 붙은 오브젝트의 태그가 "EndPoint"일 경우
            if (gameObject.CompareTag("EndPoint"))
            {
                isTriggered = true;  // 중복 방지
                StartCoroutine(LoadChattingSceneAfterDelay(2f));  // 2초 후 씬 전환
            }
        }
    }

    // 일정 시간 후 씬을 전환하는 코루틴
    private IEnumerator LoadChattingSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("chattingScene"); // chattingScene 씬 로드
    }
}
