using UnityEngine;
using System.Collections;

public class TeleportTrigger : MonoBehaviour
{
    public Transform targetPosition;
    public FadeInOutController fadeController;
    public GameObject player;
    public GameObject fadeCanvas;

    [Header("BGM Transition Settings")]
    [Tooltip("현재 BGM과 RhythmManager가 붙어있는 오브젝트 (1ChapBGM)")]
    public GameObject bgmContainer;

    // 수정: .wav 파일을 직접 연결할 수 있도록 AudioClip 타입으로 변경
    [Tooltip("교체할 새로운 배경음악 파일 (.wav, .mp3 등)")]
    public AudioClip newBGMClip;

    [Tooltip("새 BGM의 최종 목표 볼륨")]
    public float newBGMTargetVolume = 0.5f;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            if (player == null) player = other.gameObject;

            triggered = true;
            StartCoroutine(TeleportRoutine());
        }
    }

    private IEnumerator TeleportRoutine()
    {
        if (fadeCanvas != null) fadeCanvas.SetActive(true);

        // 1. 화면 페이드 아웃 (1초)
        yield return fadeController.FadeOut();

        // --- 검은 화면 유지 구간 (총 3초) 시작 ---

        AudioSource bgm_AS = null;
        RhythmManager rhythmManager = null; // RhythmManager 참조 변수 추가

        if (bgmContainer != null)
        {
            bgm_AS = bgmContainer.GetComponent<AudioSource>();
            rhythmManager = bgmContainer.GetComponent<RhythmManager>(); // RhythmManager 가져오기
        }

        // 2. 현재 BGM 볼륨 페이드 아웃 (처음 1초 동안)
        if (bgm_AS != null)
        {
            yield return fadeController.FadeOutVolume(bgm_AS);

            // 3. 새 음악 파일로 AudioSource 교체 및 재생 시작
            if (bgm_AS != null && newBGMClip != null)
            {
                bgm_AS.clip = newBGMClip;
                bgm_AS.Play();

                // RhythmManager 설정 업데이트
                if (rhythmManager != null)
                {
                    float newBPM = 179f;
                    int newCycle = 6;
                    float newBuffer = 0.74f; // 버퍼 값은 그대로 유지하거나 새 값으로 설정 가능

                    rhythmManager.UpdateRhythmSettings(newBPM, newCycle, newBuffer);
                    Debug.Log($"리듬 매니저 설정 변경: {newBPM} BPM, {newCycle} 박자");
                }
                // 모든 기믹 발판의 endBeat 값을 4로 변경
                ChangeAllPlatformTiming(4);
            }
        }


        // 4. 플레이어 이동
        if (player != null && targetPosition != null)
        {
            player.transform.position = targetPosition.position;
        }

        // 5. 남은 검은 화면 시간 대기 (3초 - 1초)
        float remainingHoldTime = fadeController.blackScreenHoldDuration - fadeController.volumeFadeDuration;
        yield return new WaitForSeconds(remainingHoldTime);

        // 6. 새 음악 볼륨 페이드 인 (1초)
        if (bgm_AS != null)
        {
            // 새 BGM의 목표 볼륨을 newBGMTargetVolume으로 설정
            yield return fadeController.FadeInVolume(bgm_AS, newBGMTargetVolume);
        }

        // 7. 화면 페이드 인 (1초)
        yield return fadeController.FadeIn();

        // --- 연출 종료 ---
        if (fadeCanvas != null) fadeCanvas.SetActive(false);
        triggered = false;
    }

    /// <summary>
    /// 씬에 있는 모든 BeatPlatformController의 endBeat 값을 변경합니다.
    /// </summary>
    private void ChangeAllPlatformTiming(int newEndBeat)
    {
        // 씬에 있는 모든 BeatPlatformController 인스턴스를 찾습니다.
        // FindObjectsByType을 사용하여 모든 활성화된 인스턴스를 가져옵니다.
        BeatPlatformController[] platforms = FindObjectsByType<BeatPlatformController>(FindObjectsSortMode.None);

        if (platforms.Length == 0)
        {
            Debug.LogWarning("씬에서 BeatPlatformController가 발견되지 않았습니다.");
            return;
        }

        foreach (BeatPlatformController platform in platforms)
        {
            platform.SetEndBeat(newEndBeat);
        }

        Debug.Log($"씬 내 {platforms.Length}개의 기믹 발판 endBeat이 {newEndBeat}으로 일괄 변경되었습니다.");
    }
}