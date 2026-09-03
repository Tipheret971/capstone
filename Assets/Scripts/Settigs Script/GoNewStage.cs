using UnityEngine;
using UnityEngine.SceneManagement;

public class GoNewStage: MonoBehaviour
{
    // 이 메서드를 버튼의 OnClick 이벤트에 연결하세요.
    public void LoadDimScene()
    {
        // "Stage01"이라는 이름의 씬으로 전환합니다.
        SceneManager.LoadScene("Stage01");
    }
}
