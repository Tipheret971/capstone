using System.Collections.Generic;
using UnityEngine;

public class BeatPlatformController : MonoBehaviour
{
    // === 에디터 설정 변수 ===
    [Header("Activation Settings")]
    [Tooltip("이 발판이 나타나기 시작할 박자 (1부터 RhythmManager의 beatsPerCycle까지)")]
    [SerializeField] private int startBeat = 1;
    [Tooltip("이 발판이 사라지기 시작할 박자 (예: 2박자 동안만 나타나게 하려면 startBeat + 2)")]
    [SerializeField] private int endBeat = 3;

    // === 내부 변수 ===
    private RhythmManager rhythmManager;
    // 수정: 하나의 Collider2D 대신, 모든 Collider2D 컴포넌트들을 담을 리스트
    private List<Collider2D> platformColliders;
    private SpriteRenderer platformRenderer; // 발판의 렌더러 (투명/색상 변경용)

    void Start()
    {
        // 1. 컴포넌트 초기화
        // GetComponent 대신 GetComponents<T>()를 사용하여 모든 Collider2D를 가져와 리스트에 저장
        platformColliders = new List<Collider2D>(GetComponents<Collider2D>());
        platformRenderer = GetComponent<SpriteRenderer>();

        // 충돌체 존재 여부 확인
        if (platformColliders == null)
        {
            Debug.LogError($"[{gameObject.name}]에 Collider2D 컴포넌트(BoxCollider2D 또는 CircleCollider2D 등)가 없습니다. 기믹 작동 불가.");
            return;
        }

        // 2. RhythmManager 찾기 (FindAnyObjectByType 사용)
        rhythmManager = FindFirstObjectByType<RhythmManager>();

        if (rhythmManager == null)
        {
            Debug.LogError("RhythmManager가 씬에 없습니다. 발판 기믹이 작동하지 않습니다.");
            return;
        }

        // 3. RhythmManager의 OnBeat 이벤트에 함수 연결 (구독)
        rhythmManager.OnBeat.AddListener(OnRhythmBeat);

        // 4. 초기 상태 설정
        CheckPlatformState(rhythmManager.currentBeatCount);
    }

    // 박자 이벤트가 발생할 때 호출되는 함수
    private void OnRhythmBeat()
    {
        CheckPlatformState(rhythmManager.currentBeatCount);
    }

    // 발판의 상태(충돌체/렌더러 활성화 여부)를 결정하는 핵심 로직
    private void CheckPlatformState(int beat)
    {
        // beatsPerCycle을 넘지 않도록 안전하게 계산 (마디를 넘어가는 구간 설정 방지)
        // int currentCycle = rhythmManager.beatsPerCycle; // 사용되지 않아 주석 처리

        // 발판이 활성화되어야 하는 조건: 현재 박자가 startBeat 이상이고 endBeat 미만일 때
        bool shouldBeActive = beat >= startBeat && beat < endBeat;

        // 혹은 마디를 넘어가는 경우 처리 (예: 4박자 마디에서 3, 4박자에 걸쳐 활성화)
        // 이 로직은 간단화를 위해 기본 범위 내에서만 처리합니다.

        SetPlatformActive(shouldBeActive);
    }

    // 발판의 활성화/비활성화 상태를 실제로 적용
    private void SetPlatformActive(bool active)
    {
        // 충돌체 활성화/비활성화 (반복문 사용)
        foreach (Collider2D collider in platformColliders)
        {
            // 리스트의 모든 Collider에 대해 활성화 상태를 변경합니다.
            if (collider.enabled != active)
            {
                collider.enabled = active;
            }
        }

        // 렌더러 활성화/비활성화
        if (platformRenderer != null)
        {
            // SpriteRenderer의 visible 상태를 활성화/비활성화
            platformRenderer.enabled = active;
        }

        // 시각적인 피드백 (선택 사항)
        if (active)
        {
            // Debug.Log($"[BeatPlatform] 나타남. Beat: {rhythmManager.currentBeatCount}");
        }
        else
        {
            // Debug.Log($"[BeatPlatform] 사라짐. Beat: {rhythmManager.currentBeatCount}");
        }
    }

    /// <summary>
    /// 발판이 사라지기 시작하는 박자(endBeat)를 변경하고 상태를 즉시 업데이트합니다.
    /// </summary>
    public void SetEndBeat(int newEndBeat)
    {
        if (endBeat == newEndBeat) return; // 값이 같으면 불필요한 연산 방지

        endBeat = newEndBeat;

        // 값이 변경된 후 현재 박자 상태를 바로 체크하여 발판 상태를 업데이트합니다.
        if (rhythmManager != null)
        {
            CheckPlatformState(rhythmManager.currentBeatCount);
        }

        Debug.Log($"[{gameObject.name}] endBeat이 {newEndBeat}으로 변경되었습니다.");
    }

    void OnDestroy()
    {
        // 스크립트나 오브젝트가 파괴될 때 이벤트 구독을 해제하여 메모리 누수 방지
        if (rhythmManager != null)
        {
            rhythmManager.OnBeat.RemoveListener(OnRhythmBeat);
        }
    }
}