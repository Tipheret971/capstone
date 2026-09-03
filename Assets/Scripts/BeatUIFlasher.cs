using UnityEngine;
using UnityEngine.UI;

public class BeatUIFlasher : MonoBehaviour
{
    [SerializeField] private RhythmManager rhythmManager; // RhythmManager 가 들어가있는 오브젝트
    [SerializeField] private Image beatIndicator;          // UI 자기 자신
    [SerializeField] private Color flashColor = Color.yellow; // 박자 시 순간 색상
    [SerializeField] private float fadeDuration = 0.2f;    // 점점 어두워지는 시간

    private Color originalColor;
    private float fadeTimer;

    void Start()
    {
        if (rhythmManager != null)
            rhythmManager.OnBeat.AddListener(OnBeat); // RhythmManager 이벤트 구독

        if (beatIndicator != null)
            originalColor = beatIndicator.color;
    }

    void Update()
    {
        // 서서히 원래 색으로 복귀
        if (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;
            float t = 1f - (fadeTimer / fadeDuration);
            beatIndicator.color = Color.Lerp(flashColor, originalColor, t);
        }
    }

    void OnBeat()
    {
        // 박자마다 순간적으로 빛나기 시작
        fadeTimer = fadeDuration;
        beatIndicator.color = flashColor;
    }
}
