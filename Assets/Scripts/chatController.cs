using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class chatController : MonoBehaviour
{
    public GameObject chatPanel;             // 전체 대화 UI 패널 (챗박스 포함, 최초에 OFF 상태)

    public TMP_Text chatText1;               // 왼쪽 인물 대화 텍스트
    public TMP_Text chatText2;               // 오른쪽 인물 대화 텍스트
    public TMP_Text chatText3;               // 상황 서술 텍스트

    public TMP_Text characterName1;          // 오른쪽 캐릭터 이름 텍스트
    public TMP_Text characterName2;          // 왼쪽 캐릭터 이름 텍스트
    public GameObject nameBox1;              // 오른쪽 이름 박스
    public GameObject nameBox2;              // 왼쪽 이름 박스

    public Image characterImage1;            // 왼쪽 캐릭터 이미지
    public Image characterImage2;            // 오른쪽 캐릭터 이미지

    private string writerText = "";
    private bool isChatting = false;

    public enum DialogueType { Left, Right, Narration } // 대화 타입 정의

    void Update()
    {
        // 테스트용: E키로 대화 시작
        if (Input.GetKeyDown(KeyCode.E) && !isChatting)
        {
            StartCoroutine(BeginConversation());
        }
    }

    IEnumerator BeginConversation()
    {
        isChatting = true;
        chatPanel.SetActive(true);

        yield return StartCoroutine(ShowDialogue(DialogueType.Narration, "", "(차가운 안개가 드리운 스틱스 강가...)"));
        yield return StartCoroutine(ShowDialogue(DialogueType.Narration, "", "(어둠 속에 뱃사공의 그림자가 드러난다.)"));
        yield return StartCoroutine(ShowDialogue(DialogueType.Left, "오르페우스", "이봐! 여긴 어디야?"));
        yield return StartCoroutine(ShowDialogue(DialogueType.Right, "카론", "산 자는 이 강을 건널 수 없다. 돌아가라."));
        yield return StartCoroutine(ShowDialogue(DialogueType.Left, "오르페우스", "난 돌아가지 않아. 에우리디케를 되찾으러 왔어!"));
        yield return StartCoroutine(ShowDialogue(DialogueType.Right, "카론", "명계는 죽은 자의 땅이다. 너의 사랑 따위는 이유가 되지 않는다."));
        yield return StartCoroutine(ShowDialogue(DialogueType.Left, "오르페우스", "제발... 내 말을 들어줘. 그 어떤 것도 그녀를 대신할 순 없어."));
        yield return StartCoroutine(ShowDialogue(DialogueType.Left, "오르페우스", "부탁이야. 내 노래를... 들어줘."));

        yield return StartCoroutine(ShowDialogue(DialogueType.Narration, "", "(오르페우스는 리라를 꺼내어 연주하기 시작한다.)"));
        yield return StartCoroutine(ShowDialogue(DialogueType.Narration, "", "(슬픔과 사랑이 담긴 선율이 강 저편으로 퍼져간다.)"));

        yield return StartCoroutine(ShowDialogue(DialogueType.Right, "카론", "...이 강은 산 자를 태우지 않는다."));
        yield return StartCoroutine(ShowDialogue(DialogueType.Right, "카론", "그러나, 네 노래는... 다른 무엇과도 달랐다."));
        yield return StartCoroutine(ShowDialogue(DialogueType.Right, "카론", "타라. 단, 돌아가는 길에 시험이 따를 것이다."));
        yield return StartCoroutine(ShowDialogue(DialogueType.Left, "오르페우스", "감사합니다. 반드시 그녀를 데리고 돌아가겠습니다."));

        chatPanel.SetActive(false);
        isChatting = false;
    }

    IEnumerator ShowDialogue(DialogueType type, string speaker, string dialogue)
    {
        ApplyDialogueType(type, speaker);

        writerText = "";
        TMP_Text targetText = GetTargetChatText(type);
        targetText.text = "";

        foreach (char c in dialogue)
        {
            writerText += c;
            targetText.text = writerText;
            yield return new WaitForSeconds(0.03f);
        }

        while (!Input.GetMouseButtonDown(0))
            yield return null;
    }

    void ApplyDialogueType(DialogueType type, string speaker)
    {
        bool isLeft = type == DialogueType.Left;
        bool isRight = type == DialogueType.Right;
        bool isNarration = type == DialogueType.Narration;

        // 모든 텍스트 비활성화 후 해당 타입만 활성화
        chatText1.gameObject.SetActive(isLeft);
        chatText2.gameObject.SetActive(isRight);
        chatText3.gameObject.SetActive(isNarration);

        // 이미지 온오프
        characterImage1.gameObject.SetActive(isLeft);
        characterImage2.gameObject.SetActive(isRight);

        // 이름박스 온오프
        nameBox1.SetActive(isRight);
        nameBox2.SetActive(isLeft);

        // 이름 텍스트 설정
        if (isLeft)
        {
            characterName2.text = speaker; // 왼쪽 발화 → 오른쪽 이름 필드
        }
        else if (isRight)
        {
            characterName1.text = speaker; // 오른쪽 발화 → 왼쪽 이름 필드
        }
    }

    TMP_Text GetTargetChatText(DialogueType type)
    {
        switch (type)
        {
            case DialogueType.Left:
                return chatText1;
            case DialogueType.Right:
                return chatText2;
            case DialogueType.Narration:
                return chatText3;
            default:
                return null;
        }
    }
}
