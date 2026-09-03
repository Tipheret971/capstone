using UnityEngine;

public class LiraBeatGlow : MonoBehaviour
{
    [Header("Beat Glow Settings")]
    public Color baseColor = new Color(0.6f, 0.8f, 1f); // 평소 색
    public Color beatColor = Color.white; // 박자 시 색상
    public float flashDuration = 0.15f; // 빛 지속 시간
    public float scaleBoost = 1.1f; // 박자 때 크기 확대 비율

    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;
    private bool isFlashing = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;

        // RhythmManager 이벤트 구독
        RhythmManager manager = FindObjectOfType<RhythmManager>();
        if (manager != null)
            manager.OnBeat.AddListener(OnBeat);
        else
            Debug.LogWarning("⚠️ RhythmManager를 찾을 수 없습니다. 리라 박자 반응 비활성화됨.");
    }

    void OnBeat()
    {
        if (!isFlashing)
            StartCoroutine(FlashRoutine());
    }

    System.Collections.IEnumerator FlashRoutine()
    {
        isFlashing = true;

        // 색상과 크기 변화
        spriteRenderer.color = beatColor;
        transform.localScale = originalScale * scaleBoost;

        yield return new WaitForSeconds(flashDuration);

        // 원래 상태 복귀
        spriteRenderer.color = baseColor;
        transform.localScale = originalScale;

        isFlashing = false;
    }
}
