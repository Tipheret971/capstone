using UnityEngine;
using UnityEngine.Events;

public class RhythmManager : MonoBehaviour
{
    [Header("Rhythm Settings")]
    [SerializeField] private float bpm = 80f; // 음악의 BPM (Beats Per Minute, 분당 박자 수)
    [SerializeField] public int beatsPerCycle = 4; // 한 마디(사이클) 당 박자 수
    public float jumpWindowDuration = 0.2f; // 점프 허용 구간(초)
    public float buffer = 0.0f; //버퍼링 조정

    private float beatInterval; // 박자 간격(초) = 60 / BPM
    private float nextBeatTime; // 다음 박자가 발생할 시점(Time.time 기준)
    private float lastBeatTime; // 마지막 박자가 발생한 시간(Time.time 기준)
    public int currentBeatCount { get; private set; } = 0; // 현재 박자 번호(1~beatsPerCycle)

    [Header("Rhythm Events")]
    public UnityEvent OnBeat; // 박자가 발생할 때 호출할 이벤트, 외부에서 리스너 추가 가능

    private AudioSource audioSource; // 배경음악 AudioSource

    void Start()
    {
        beatInterval = 60f / bpm;
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.Play();
            // 오디오 출력 버퍼링 보정 (0.1~0.2초 정도)
            nextBeatTime = Time.time + beatInterval + buffer;
        }
        else
        {
            nextBeatTime = Time.time + beatInterval;
        }

        lastBeatTime = Time.time;
    }

    // ... (Update() 함수는 이전과 동일) ...
    void Update()
    {
        // Time.time이 nextBeatTime 이상이면 박자가 발생
        while (Time.time >= nextBeatTime)
        {
            // 1. 현재 박자 증가
            currentBeatCount++;
            if (currentBeatCount > beatsPerCycle)
                currentBeatCount = 1; // 사이클 끝나면 다시 1로 초기화

            // 2. 박자 간 실제 간격 계산
            float intervalSinceLastBeat = Time.time - lastBeatTime;
            lastBeatTime = Time.time; // 마지막 박자 시간 갱신

            // 3. 예정 박자 시간과 실제 시간 차이 계산 (오차 측정용)
            float delta = Time.time - nextBeatTime;

            // 4. 디버그 로그 출력
            Debug.Log($"🟢 [{Time.time:F3}s] {currentBeatCount}번째 박자 발생! (오차: {delta:+0.000;-0.000}s, 간격: {intervalSinceLastBeat:F3}s)");

            // 5. 박자 이벤트 호출 (외부 스크립트에서 리스너 구독 가능)
            OnBeat.Invoke();

            // 6. 다음 박자 시간 계산
            nextBeatTime += beatInterval;

            // 안전 장치: beatInterval이 0 이하일 경우 무한 루프 방지
            if (beatInterval <= 0f) break;
        }
    }

    // ... (IsDoubleJumpAllowed() 함수는 이전과 동일) ...
    public bool IsDoubleJumpAllowed()
    {
        // 다음 박자까지 남은 시간
        float timeToNextBeat = nextBeatTime - Time.time;
        // 마지막 박자 이후 경과 시간
        float timeSinceLastBeat = beatInterval - timeToNextBeat;
        // 허용 구간 안에 있는지 체크
        bool inWindow = timeSinceLastBeat >= 0f && timeSinceLastBeat <= jumpWindowDuration;

        // 허용 구간일 경우 디버그 로그 출력
        if (inWindow)
            Debug.Log($"🟡 점프 가능 구간! beat {currentBeatCount} | timeSinceLastBeat={timeSinceLastBeat:F3}");

        return inWindow; // 점프 가능 여부 반환
    }

    // =======================================================
    // 새 리듬 설정 업데이트 메서드 추가
    // =======================================================

    /// <summary>
    /// 새로운 음악의 BPM, 박자 수, 버퍼를 설정하고 리듬 타이머를 재설정합니다.
    /// </summary>
    /// <param name="newBPM">새로운 BPM 값</param>
    /// <param name="newBeatsPerCycle">새로운 한 마디 당 박자 수</param>
    /// <param name="newBuffer">새로운 버퍼 값 (선택 사항)</param>
    public void UpdateRhythmSettings(float newBPM, int newBeatsPerCycle, float newBuffer = 0f)
    {
        if (newBPM <= 0)
        {
            Debug.LogError("BPM은 0보다 커야 합니다. 설정 변경 불가.");
            return;
        }

        // 1. 새로운 설정값 적용
        bpm = newBPM;
        beatsPerCycle = newBeatsPerCycle;
        buffer = newBuffer;

        // 2. 박자 간격 재계산
        beatInterval = 60f / bpm;

        // 3. 현재 박자 초기화
        currentBeatCount = 0;

        // 4. 다음 박자 시간 재설정
        // 현재 Time.time에 새로운 박자 간격(beatInterval)을 더하여 즉시 새 리듬에 동기화
        nextBeatTime = Time.time + beatInterval + buffer;
        lastBeatTime = Time.time;

        Debug.Log($"[RhythmManager] 리듬 설정 업데이트 완료: BPM={bpm}, Cycle={beatsPerCycle}, Interval={beatInterval:F3}s");
    }

    // RhythmManager의 BPM 값을 반환하는 공개 프로퍼티 또는 메서드
    public float GetBPM()
    {
        return bpm;
    }
}