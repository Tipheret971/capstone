using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOneWayPlatform : MonoBehaviour
{
    public bool IsFacingLeft => spriteRenderer.flipX;
    private GameObject currentOneWayPlatform; // 현재 올라가 있는 원웨이 플랫폼
    [SerializeField] private BoxCollider2D playerCollider; // 플레이어의 Collider (플랫폼과의 충돌 처리용)
    private Rigidbody2D rb; // 플레이어의 Rigidbody2D
    private float moveInput; // 플레이어의 이동 입력 (좌우 방향)
    private bool isGrounded; // 플레이어가 땅에 닿아있는지 확인하는 변수
    private bool isOnJumpPlatform; // 점프 플랫폼 위에 있는지 여부
    public float moveSpeed = 8f; // 이동 속도
    public float jumpForce = 8f; // 점프 힘
    public Transform groundCheck; // 땅에 닿았는지 체크하는 위치 (플레이어 발 아래)
    public LayerMask groundLayer; // 땅을 체크할 레이어
    public float fallMultiplier = 2.5f; // 빠른 하강을 위한 중력 배율
    public LayerMask jumpPlatformLayer; // 점프 플랫폼 레이어
    private bool isInDoubleJumpZone = false; // 더블점프 가능상태
    private bool hasUsedDoubleJump = false; // 공중에서 리듬 점프 사용 여부

    // Knockback (튕겨나기) 관련 변수
    private bool isKnockback = false; // 현재 튕겨나가고 있는지 여부
    private float knockbackDuration = 0.6f; // 튕겨나가는 시간
    private float knockbackTimer = 0f; // 튕겨나가는 타이머

    // 무적 처리 관련 변수
    private bool isInvincible = false; // 무적 상태 여부
    private float invincibleDuration = 1.0f; // 무적 지속 시간
    private float invincibleTimer = 0f; // 무적 타이머

    public SpriteRenderer spriteRenderer; // 플레이어의 SpriteRenderer (색상 변경을 위해)
    private Coroutine flickerCoroutine; // 무적 시 깜빡임 효과를 위한 코루틴 변수
    private Vector2 groundCheckSize = new Vector2(0.96f, 0.2f); // 그라운드 체크 범위
    public float maxFallSpeed = 10f; // 최대 낙하 속도

    private PlayerHealth playerHealth;
    private bool canBeKnockbacked = true;
    private bool isJumping;
    private bool isFalling;
    private bool jumpInput;
    private Animator animator;
    private bool canReverseGravity = false; // 중력 반전 가능 여부
    private bool isGravityReversed = false; // 현재 중력이 반전된 상태인지 여부

    //[SerializeField] private AudioSource walkAudioSource; // 걷기 루프 사운드 //!!!!! 디버그 로그 에러때문에 임시로 지워둠

    [SerializeField] private AudioClip jumpClip; // 점프 사운드 클립
    [SerializeField] private AudioSource sfxAudioSource; // 효과음 재생용

    public float rhythmCycle = 1f; // 리듬 주기
    public float jumpWindowDuration = 0.2f; // 추가 점프 가능 시간
    private float currentCycleTime = 0f; // 현재 리듬 사이클
    private RhythmManager rhythmManager; // 새로 추가: RhythmManager 참조 변수

    // === 새 변수 추가 ===
    private BeatMovingPlatform currentPlatform; // 현재 탑승 중인 발판

    private bool inputEnabled = true; // 현재 입력 활성화 상태

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 컴포넌트를 가져옴
        spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer 컴포넌트를 가져옴
        animator = GetComponent<Animator>(); // Animator 연결

        if (rb == null) Debug.LogError("Rigidbody2D가 없습니다!");
        if (spriteRenderer == null) Debug.LogError("SpriteRenderer가 없습니다!");

        // RhythmManager 연결
        rhythmManager = FindFirstObjectByType<RhythmManager>();
        if (rhythmManager == null) Debug.LogError("RhythmManager가 씬에 없습니다!");
    }

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        // 튕겨나가고 있으면 입력을 받지 않음
        if (isKnockback) return;

        if (inputEnabled)
        {
            // 아래로 원웨이 플랫폼 뚫기
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentOneWayPlatform != null)
                {
                    StartCoroutine(DisableCollison());
                }
            }

            // 캐릭터 애니메이션 좌우 반전
            moveInput = Input.GetAxisRaw("Horizontal");
            if (moveInput > 0) spriteRenderer.flipX = false; // 오른쪽 바라봄
            else if (moveInput < 0) spriteRenderer.flipX = true; // 왼쪽 바라봄
            animator.SetFloat("Speed", Mathf.Abs(moveInput));

            // 좌우 이동 입력 받기
            moveInput = Input.GetAxisRaw("Horizontal");
            if (animator != null) animator.SetFloat("Speed", Mathf.Abs(moveInput));

            

            // === 리듬 타이머 로직 제거 (RhythmManager로 이동) ===
            // currentCycleTime += Time.deltaTime;
            // if (currentCycleTime >= rhythmCycle)
            // {
            //     currentCycleTime -= rhythmCycle;
            // }

            // CheckDoubleJumpWindow(); // 제거

            float jumpDir = isGravityReversed ? -1f : 1f;

            // 1. 리듬 기반 추가 점프
            // 조건: 공중 + 점프 입력 + 미사용 + DoubleJumpZone 내 + 리듬 타이밍 적합 (RhythmManager.IsDoubleJumpAllowed())
            if (!isGrounded && Input.GetButtonDown("Jump") && !hasUsedDoubleJump && isInDoubleJumpZone && rhythmManager != null && rhythmManager.IsDoubleJumpAllowed())
            {
                // 수직 속도 초기화 후 새로운 힘을 적용
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * jumpDir);

                hasUsedDoubleJump = true; // 추가 점프 사용 처리
                animator.SetBool("Jump", true);
                sfxAudioSource.PlayOneShot(jumpClip, 2.0f); // 점프 효과음 재생
                return; // 추가 점프가 실행되면 아래 일반 점프 로직은 무시
            }

            // 2. 일반 점프 (땅에 닿았을 때)
            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * jumpDir);
                animator.SetBool("Jump", true);
                sfxAudioSource.PlayOneShot(jumpClip, 2.0f); // 점프 효과음 재생
            }

            if (!isGrounded) animator.SetBool("Jump", false);

            // 점프 플랫폼 자동 점프
            if (isOnJumpPlatform)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 1.5f * jumpDir);
            }

            // 중력 반전
            if (canReverseGravity && Input.GetKeyDown(KeyCode.Space))
            {
                isGravityReversed = !isGravityReversed;
                rb.gravityScale *= -1;
                Vector3 localScale = transform.localScale;
                localScale.y *= -1;
                transform.localScale = localScale;
            }

            // 걷기 사운드 제어
            bool isMovingHorizontally = moveInput != 0f;
            bool shouldPlayWalkSound = isMovingHorizontally && isGrounded;
            //if (shouldPlayWalkSound && !walkAudioSource.isPlaying) walkAudioSource.Play();
            //else if ((!shouldPlayWalkSound || !isGrounded) && walkAudioSource.isPlaying) walkAudioSource.Stop();
        }
        else // inputEnabled == false (입력이 막힌 상태)
        {
            // [핵심 수정] 입력이 막힌 상태에서는 moveInput을 0으로 강제합니다.
            moveInput = 0f;
            // 애니메이션 속도도 0으로 설정하여 캐릭터가 즉시 멈춰 보이도록 합니다.
            if (animator != null) animator.SetFloat("Speed", 0f);

            // 입력이 비활성화되었으므로, 이 블록 아래의 모든 Input.GetKeyDown/GetButtonDown 로직은
            // inputEnabled 블록으로 옮겨야 합니다. (위에서 이미 처리 완료)
        }

        

        // 무적 시간이 남아있으면 타이머 감소
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
            {
                isInvincible = false;
                if (flickerCoroutine != null)
                {
                    StopCoroutine(flickerCoroutine);
                    spriteRenderer.color = Color.white;
                }
            }
        }

        // 땅에 닿았는지 확인
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
        isOnJumpPlatform = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, jumpPlatformLayer);

        // 땅에 닿았을 때 추가 점프 사용 상태 초기화
        if (isGrounded)
        {
            hasUsedDoubleJump = false;
        }
    }

    private void FixedUpdate()
    {
        // if (!isKnockback) rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        if (!isKnockback)
        {
            Vector2 platformVelocity = Vector2.zero;

            // 현재 발판 위에 있다면 발판의 속도를 가져옵니다.
            if (isGrounded && currentPlatform != null)
            {
                platformVelocity = currentPlatform.CurrentVelocity;
            }

            // 1. 수평 속도 계산 (moveInput은 Update에서 이미 0으로 제어됨)
            float targetXVelocity = moveInput * moveSpeed + platformVelocity.x;

            // 2. 수직 속도 처리 (튀는 현상 방지 로직)
            float targetYVelocity = rb.linearVelocity.y;

            if (isGrounded && currentPlatform != null)
            {
                // 발판 위에 있고, 발판이 위로 움직이거나 정지했을 때 (튀는 현상 방지)
                // 플레이어의 Y 속도가 0이거나 하강 중일 때만 발판 속도로 덮어씌웁니다.
                if (targetYVelocity <= 0f)
                {
                    // 발판의 Y 속도로 강제 적용
                    targetYVelocity = platformVelocity.y;
                }
                // 참고: 플레이어가 점프 중일 때 (targetYVelocity > 0)는 점프 속도를 유지합니다.
            }

            // 최종 속도 적용
            rb.linearVelocity = new Vector2(targetXVelocity, targetYVelocity);
        }

        // 중력 반전 여부에 따라 낙하 판별
        bool isFalling = isGravityReversed ? rb.linearVelocity.y > 0 : rb.linearVelocity.y < 0;
        if (isFalling)
        {
            float gravityDirection = isGravityReversed ? -1f : 1f;
            rb.linearVelocity += Vector2.up * gravityDirection * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        // 낙하 속도 제한 적용
        if (!isKnockback)
        {
            float clampedY = isGravityReversed
                ? Mathf.Clamp(rb.linearVelocity.y, -float.MaxValue, maxFallSpeed)
                : Mathf.Clamp(rb.linearVelocity.y, -maxFallSpeed, float.MaxValue);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, clampedY);
        }

        // Knockback 처리
        if (isKnockback)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f) isKnockback = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 장애물과 충돌 시, 무적이 아니면 튕겨나가기
        if (collision.gameObject.CompareTag("Obstacle") && !isInvincible && canBeKnockbacked)
        {
            playerHealth.TakeDamage(1); // 피해 1
            Vector2 knockbackDir = (transform.position - collision.transform.position).normalized;
            knockbackDir.y = Mathf.Clamp(knockbackDir.y, 0.5f, 1f); // Y 방향은 너무 커지지 않도록 제한
            rb.linearVelocity = Vector2.zero; // 속도 초기화
            rb.AddForce(knockbackDir * 7f, ForceMode2D.Impulse); // 튕겨나가기
            isKnockback = true; // 튕겨나가는 상태로 변경
            knockbackTimer = knockbackDuration; // 튕겨나기 시간 설정
            isInvincible = true; // 무적 상태로 변경
            invincibleTimer = invincibleDuration; // 무적 시간 설정
            // 깜빡임 코루틴 시작
            if (flickerCoroutine != null) StopCoroutine(flickerCoroutine);
            flickerCoroutine = StartCoroutine(Flicker());
        }

        // 원웨이 플랫폼에 올라갔을 때
        if (collision.gameObject.CompareTag("OneWayPlatform")) currentOneWayPlatform = collision.gameObject;

        // 움직이는 발판에 닿았을 때 ("MovingPlatform" 태그를 사용한다고 가정)
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            // **[수정]** 부모 설정 로직 제거: transform.SetParent(collision.transform);

            // 발판 컴포넌트를 가져와 저장
            currentPlatform = collision.gameObject.GetComponent<BeatMovingPlatform>();

            // (선택 사항) 벽에 붙는 문제를 해결하기 위해 마찰력이 0이 아니라 
            // 0.2~0.5 정도의 Physics Material을 사용하고 이 로직을 적용하는 것이 이상적입니다.
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // 원웨이 플랫폼에서 내려갔을 때
        if (collision.gameObject.CompareTag("OneWayPlatform")) currentOneWayPlatform = null;

        // 움직이는 발판에서 벗어났을 때
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            // **[수정]** 부모 해제 로직 제거: transform.SetParent(null);

            // 현재 탑승 중인 발판 참조 제거
            currentPlatform = null;
        }
    }

    // 원웨이 플랫폼과의 충돌을 비활성화하는 코루틴
    private IEnumerator DisableCollison()
    {
        BoxCollider2D platformCollider = currentOneWayPlatform.GetComponent<BoxCollider2D>(); // 원웨이 플랫폼의 Collider 가져오기
        Physics2D.IgnoreCollision(playerCollider, platformCollider); // 충돌 무시
        yield return new WaitForSeconds(0.5f); // 0.5초 대기
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false); // 충돌 다시 활성화
    }

    // 무적 시 깜빡이는 효과
    private IEnumerator Flicker()
    {
        while (isInvincible)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f); // 반투명 효과
            yield return new WaitForSeconds(0.1f); // 0.1초 대기
            spriteRenderer.color = Color.white; // 색상 원복
            yield return new WaitForSeconds(0.1f); // 0.1초 대기
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ReserveGravity"))
        {
            canReverseGravity = true; // 중력 반전 가능 영역 진입
        }
        if (other.CompareTag("DeathZone"))
        {
            playerHealth.TakeDamage(3); // 즉시 리스폰
        }
        if (other.CompareTag("DoubleJumpZone"))
        {
            isInDoubleJumpZone = true; // 더블 점프 활성화
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ReserveGravity"))
        {
            canReverseGravity = false; // 중력 반전 영역에서 나감
        }
        // [수정] DoubleJumpZone 태그를 벗어났을 때 비활성화
        if (other.CompareTag("DoubleJumpZone"))
        {
            isInDoubleJumpZone = false; // 더블 점프 비활성화
            // 영역을 벗어나면 사용 여부도 리셋해주는 것이 안전할 수 있습니다.
            hasUsedDoubleJump = false;
        }
    }

    // 더블 점프 가능여부 판단
    // private bool IsDoubleJumpAllowed()
    // {
    //     // isInDoubleJumpZone 이 true 일 때만
    //     if (!isInDoubleJumpZone) return false;
    // 
    //     // 현재 시간이 점프 가능한 시간 범위(0 ~ 0.2초) 내에 있는지 확인
    //     return currentCycleTime <= jumpWindowDuration;
    // }

    public void DisableKnockback() { canBeKnockbacked = false; }
    public void EnableKnockback() { canBeKnockbacked = true; }

    public void ResetState()
    {
        // 입력 상태 초기화 등 필요한 것들
        isJumping = false;
        isFalling = false;
        jumpInput = false;
        // velocity 관련 입력 타이밍 등도 필요하면 초기화
        hasUsedDoubleJump = false; // 추가 점프 상태 초기화
    }

    public void ResetKnockbackAndInvincibility()
    {
        isKnockback = false;
        knockbackTimer = 0f;
        isInvincible = false;
        invincibleTimer = 0f;
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }
        spriteRenderer.color = Color.white;
    }

    public void ResetGravityState()
    {
        // 리스폰시 중력 정상화를 위해 사용
        if (isGravityReversed)
        {
            isGravityReversed = false;
            rb.gravityScale = Mathf.Abs(rb.gravityScale); // 정상 방향으로
            Vector3 localScale = transform.localScale;
            localScale.y = Mathf.Abs(localScale.y); // 캐릭터도 위로 다시 정렬
            transform.localScale = localScale;
        }
    }

    // DialogueManager가 호출할 공개 함수
    public void SetInputEnabled(bool isEnabled)
    {
        inputEnabled = isEnabled;

        // [핵심 수정] 입력이 막힐 때 플레이어의 관성 이동을 즉시 멈춥니다.
        if (!isEnabled)
        {
            // Rigidbody2D rb = GetComponent<Rigidbody2D>(); // Start에서 이미 가져옴
            if (rb != null)
            {
                // X 속도만 0으로 만들고, Y 속도(중력/낙하)는 유지
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
    }
}