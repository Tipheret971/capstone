using System.Collections;
using UnityEngine;

public class PlayerRespawner : MonoBehaviour
{
    [SerializeField] private Transform respawnTransform;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private PlayerOneWayPlatform playerScript;
    [SerializeField] private FadeInOutController fadeController;
    public GameObject fadeCanvas; // 추가: FadeCanvas 참조

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        playerScript = GetComponent<PlayerOneWayPlatform>();
    }

    public void SetRespawnPoint(Transform newRespawnPoint)
    {
        respawnTransform = newRespawnPoint;
    }

    public void Respawn()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        // 0. 화면 페이드 아웃
        fadeCanvas.SetActive(true);
        if (fadeController != null)
            yield return StartCoroutine(fadeController.FadeOut());

        // 1. 물리 충돌 및 속도 제거
        playerCollider.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        yield return new WaitForSeconds(0.05f);

        // 2. 위치 이동
        transform.position = respawnTransform.position;

        // 3. 플레이어 입력 상태 초기화
        if (playerScript != null)
        {
            playerScript.ResetState();
            playerScript.ResetGravityState();
        }

        yield return new WaitForSeconds(0.05f);

        // 4. 다시 물리 활성화
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
        playerCollider.enabled = true;

        // 5. 화면 페이드 인
        if (fadeController != null)
            yield return StartCoroutine(fadeController.FadeIn());
        fadeCanvas.SetActive(false);
    }
}