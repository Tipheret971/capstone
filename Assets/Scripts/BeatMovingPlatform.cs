using UnityEngine;

public class BeatMovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("현재 위치를 기준으로 이동할 목표 X 거리")]
    [SerializeField] private float moveXDistance = 2f;
    [Tooltip("현재 위치를 기준으로 이동할 목표 Y 거리")]
    [SerializeField] private float moveYDistance = 0f;
    [Tooltip("이동 시작 후 목표 위치에 도달하는 데 걸리는 박자 수")]
    [SerializeField] private int beatsToTarget = 1; // 1마디(Cycle) 동안 이동할 것이므로, 기본값은 1로 설정

    // === 내부 변수 ===
    private RhythmManager rhythmManager;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 currentTargetPosition; // 현재 이동 중인 목표 위치 (원래 위치 또는 계산된 목표 위치)

    private bool isMovingToTarget = true; // 현재 목표 위치(targetPosition)로 이동 중인가?
    private float moveTimer = 0f; // 현재 이동 사이클의 타이머 (0.0 ~ 1.0)
    private float currentCycleDuration; // 현재 리듬 사이클의 총 시간 (RhythmManager에서 가져옴)

    private Vector3 previousPosition; // 직전 프레임의 위치
    public Vector3 CurrentVelocity { get; private set; } // 현재 프레임의 속도/이동 벡터

    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + new Vector3(moveXDistance, moveYDistance, 0);
        currentTargetPosition = startPosition;

        rhythmManager = FindFirstObjectByType<RhythmManager>();

        if (rhythmManager == null)
        {
            Debug.LogError("RhythmManager가 씬에 없습니다. 움직이는 발판이 작동하지 않습니다.");
            return;
        }

        // 초기 사이클 시간 계산 (Start 시점의 BPM 기준)
        UpdateCycleDuration(rhythmManager.beatsPerCycle);

        // RhythmManager의 OnBeat 이벤트에 마디 시작 시 호출될 함수 연결
        rhythmManager.OnBeat.AddListener(OnRhythmBeat);

        previousPosition = transform.position; // 초기 위치 저장
    }

    void Update()
    {
        // 1. 이동 타이머 업데이트
        if (currentCycleDuration > 0)
        {
            // 타이머를 현재 마디 시간에 맞춰 0부터 1까지 Lerp 값으로 만듭니다.
            moveTimer += Time.deltaTime / currentCycleDuration;
            moveTimer = Mathf.Clamp01(moveTimer);
        }

        // 2. 부드러운 이동 처리
        Vector3 from;
        Vector3 to;

        if (isMovingToTarget)
        {
            // 목표 위치로 이동 중: startPosition -> targetPosition
            from = startPosition;
            to = targetPosition;
        }
        else
        {
            // 원래 위치로 복귀 중: targetPosition -> startPosition
            from = targetPosition;
            to = startPosition;
        }

        // Lerp 함수를 사용하여 부드럽게 이동
        transform.position = Vector3.Lerp(from, to, moveTimer);

        // 3. 현재 이동 벡터 계산 (FixedUpdate에서 하는 것이 더 정확하지만, Update에서도 가능)
        CurrentVelocity = (transform.position - previousPosition) / Time.deltaTime;

        // 4. 직전 위치 업데이트
        previousPosition = transform.position;
    }

    private void OnRhythmBeat()
    {
        // 마디가 시작될 때 (currentBeatCount가 1일 때) 이동 방향을 전환합니다.
        if (rhythmManager.currentBeatCount == 1)
        {
            // 1. 방향 전환
            // isMovingToTarget이 false면 (복귀 중이었다면) true로 (이동 시작)
            // isMovingToTarget이 true면 (이동 중이었다면) false로 (복귀 시작)
            isMovingToTarget = !isMovingToTarget;

            // 2. 타이머 초기화 (새로운 이동 사이클 시작)
            moveTimer = 0f;

            // 3. 리듬 변경에 대비하여 사이클 시간 업데이트
            UpdateCycleDuration(rhythmManager.beatsPerCycle);
        }
    }

    /// <summary>
    /// 현재 BPM 및 beatsPerCycle을 기반으로 한 마디의 총 시간을 계산합니다.
    /// </summary>
    private void UpdateCycleDuration(int beatsInCycle)
    {
        // 수정: GetBPM() 메서드를 호출하여 BPM 값을 안전하게 가져옵니다.
        float bpm = rhythmManager.GetBPM();

        float beatInterval = 60f / bpm;

        // 한 마디의 총 시간 = 박자 간격 * 마디 내 박자 수
        currentCycleDuration = beatInterval * beatsInCycle;
    }

    void OnDestroy()
    {
        if (rhythmManager != null)
        {
            rhythmManager.OnBeat.RemoveListener(OnRhythmBeat);
        }
    }
}