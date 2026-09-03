using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용한다면 이 네임스페이스를 사용해야 합니다.
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialoguePanel; // 텍스트 박스를 포함하는 전체 패널/텍스트 박스 GameObject
    public Button fullScreenButton; // 화면 전체를 덮는 투명한 버튼
    public TMP_Text dialogueText; // 텍스트를 표시할 TextMeshPro 컴포넌트

    // 대화 목록을 순서대로 저장할 변수
    private Queue<string> sentences;
    private bool isDialogueActive = false; // 대화 진행 중 여부

    // 키보드 입력을 제어할 플레이어 컨트롤러 스크립트 (사용자가 별도로 구현해야 함)
    private PlayerOneWayPlatform playerController;

    void Awake()
    {
        sentences = new Queue<string>();

        // 씬에서 PlayerController를 찾아 저장 (플레이어 이동 제어용)
        playerController = FindFirstObjectByType<PlayerOneWayPlatform>();

        // 초기 상태 설정
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (fullScreenButton != null) fullScreenButton.gameObject.SetActive(false);

        // 버튼 클릭 리스너 연결
        if (fullScreenButton != null)
        {
            fullScreenButton.onClick.AddListener(DisplayNextSentence);
        }
    }

    /// <summary>
    /// 대화를 시작하고 UI를 활성화합니다.
    /// </summary>
    /// <param name="dialogues">표시할 텍스트 목록</param>
    public void StartDialogue(string[] dialogues)
    {
        if (isDialogueActive) return; // 이미 대화 중이라면 무시

        isDialogueActive = true;

        // 1. 플레이어 입력 비활성화
        if (playerController != null) playerController.SetInputEnabled(false);

        // 2. UI 활성화
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (fullScreenButton != null) fullScreenButton.gameObject.SetActive(true);

        // 3. 기존 대화 내용 제거 및 새 목록 추가
        sentences.Clear();
        foreach (string sentence in dialogues)
        {
            sentences.Enqueue(sentence);
        }

        Debug.Log("대화 시작: " + dialogues.Length + "개의 문장.");

        // 첫 번째 문장 표시
        DisplayNextSentence();
    }

    /// <summary>
    /// 다음 문장을 표시하거나 대화를 종료합니다. (버튼 클릭 시 호출)
    /// </summary>
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();

        // 텍스트 표시
        if (dialogueText != null)
        {
            dialogueText.text = sentence;
        }

        // (참고: 필요하다면 여기서 StartCoroutine(TypeSentence(sentence)); 를 사용하여 타이핑 효과를 구현할 수 있습니다.)
    }

    /// <summary>
    /// 대화를 종료하고 UI를 비활성화합니다.
    /// </summary>
    void EndDialogue()
    {
        isDialogueActive = false;

        // 1. UI 비활성화
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (fullScreenButton != null) fullScreenButton.gameObject.SetActive(false);

        // 2. 플레이어 입력 활성화
        if (playerController != null) playerController.SetInputEnabled(true);

        Debug.Log("대화 종료.");
    }
}