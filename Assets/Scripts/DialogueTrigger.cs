using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Content")]
    [TextArea(3, 10)]
    public string[] dialogueSentences; // 인스펙터에서 입력할 텍스트 목록

    private DialogueManager manager;
    private bool hasTriggered = false; // 대화가 이미 실행되었는지 확인 (선택 사항)

    void Start()
    {
        // 씬에서 DialogueManager를 찾습니다.
        manager = FindFirstObjectByType<DialogueManager>();

        if (manager == null)
        {
            Debug.LogError("DialogueManager가 씬에 없습니다! 대화 시스템이 작동하지 않습니다.");
        }
    }

    // 플레이어가 충돌 영역에 들어왔을 때 실행됩니다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // "Player" 태그를 가진 오브젝트와 충돌했고, DialogueManager가 준비되었으며, 아직 실행되지 않았다면
        if (manager != null && other.CompareTag("Player") && !hasTriggered)
        {
            manager.StartDialogue(dialogueSentences);
            hasTriggered = true; // 대화를 한 번만 실행하고 싶다면 이 주석을 해제하세요.
        }
    }

    // (선택 사항) 대화가 한 번 실행된 후 플레이어가 나가면 다시 활성화하고 싶다면
    /*
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hasTriggered)
        {
            hasTriggered = false; 
        }
    }
    */
}