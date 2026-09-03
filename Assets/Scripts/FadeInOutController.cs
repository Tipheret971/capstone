using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeInOutController : MonoBehaviour
{
    public Image fadeImage;

    [Header("Fade Timing")]
    [Tooltip("화면 전환에 걸리는 시간 (1.0f 권장)")]
    public float fadeTransitionDuration = 1.0f;

    [Tooltip("완전히 검은 화면 유지 시간 (음악 페이드 시간 포함)")]
    public float blackScreenHoldDuration = 3.0f; // 3초로 기준 설정

    // [Header("Audio Settings")]
    // public AudioSource bgmAudioSource; // RhythmManager의 AudioSource를 연결
    // public AudioClip newBGMClip; // 교체할 새로운 배경음악 클립
    [Tooltip("볼륨이 0으로 줄어드는 시간 (1초로 고정)")]
    public float volumeFadeDuration = 1.0f; // 볼륨 페이드 아웃/인 속도

    // 현재 볼륨을 저장하여 복구할 때 사용
    private float originalVolume;

    void Start()
    {
        // 초기 상태: 화면이 투명한 상태 (Alpha=0)
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
        if (fadeImage != null) fadeImage.gameObject.SetActive(true);

        // BGM 관련 초기화 로직은 이제 필요 없습니다. (AudioSource를 인수로 받기 때문)
        // if (bgmAudioSource == null) { ... }
        // if (bgmAudioSource != null) { originalVolume = bgmAudioSource.volume; } 
    }

    // ... (Fade(targetAlpha) 함수는 이전과 동일) ...
    private IEnumerator Fade(float targetAlpha)
    {
        float duration = fadeTransitionDuration;
        float startAlpha = fadeImage.color.a;
        float t = 0;
        Color c = fadeImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = targetAlpha;
        fadeImage.color = c;
    }


    // --- 새로운 음악 제어 코루틴 ---

    // 볼륨을 0으로 줄이는 코루틴. AudioSource를 인수로 받음.
    public IEnumerator FadeOutVolume(AudioSource audioSource)
    {
        if (audioSource == null) yield break;

        float t = 0;
        float originalVolume = audioSource.volume; // 현재 볼륨을 시작 볼륨으로 사용

        while (t < volumeFadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(originalVolume, 0, t / volumeFadeDuration);
            yield return null;
        }
        audioSource.volume = 0f; // 볼륨 최종 0 고정
    }

    // 볼륨을 목표치로 회복시키는 코루틴. AudioSource와 목표 볼륨을 인수로 받음.
    public IEnumerator FadeInVolume(AudioSource audioSource, float targetVolume)
    {
        if (audioSource == null) yield break;

        float t = 0;
        float startVolume = audioSource.volume; // 시작 볼륨은 0일 것입니다.

        while (t < volumeFadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t / volumeFadeDuration);
            yield return null;
        }
        audioSource.volume = targetVolume; // 볼륨 최종 복구
    }


    // 기존 FadeOut/FadeIn 코루틴 (호환성 유지)
    public IEnumerator FadeOut()
    {
        yield return StartCoroutine(Fade(1));
    }
    public IEnumerator FadeIn()
    {
        yield return StartCoroutine(Fade(0));
    }
}