using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0.5f, 0.3f, 0); // x 오프셋은 오른쪽 기준
    public float followSpeed = 5f;

    private PlayerOneWayPlatform playerScript;

    void Start()
    {
        playerScript = target.GetComponent<PlayerOneWayPlatform>();
    }

    void LateUpdate()
    {
        if (target == null || playerScript == null) return;

        // 👇 좌우 반전 계산을 반대로
        float dir = playerScript.IsFacingLeft ? 1f : -1f;
        Vector3 dynamicOffset = new Vector3(offset.x * dir, offset.y, offset.z);

        Vector3 desiredPosition = target.position + dynamicOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }
}
