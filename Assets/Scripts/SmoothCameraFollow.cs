using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform player;                        // 따라갈 대상
    public Vector3 offset = new Vector3(0, 0, -10); // 기본 오프셋 (2D에서는 Z = -10 추천)
    public float smoothTime = 0.3f;                 // 부드러움 정도 (클수록 느림)

    private Vector3 velocity = Vector3.zero;        // 내부적으로 사용하는 속도

    void LateUpdate()
    {
        Vector3 targetPosition = player.position + offset; // 목표 위치
        // 부드럽고 탄성 있는 이동
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}