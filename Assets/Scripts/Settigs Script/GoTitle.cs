using UnityEngine;
using UnityEngine.SceneManagement;

public class GoTitle : MonoBehaviour
{
    // 이 메서드를 버튼의 OnClick 이벤트에 연결하세요.
    public void LoadTitleScene()
    {
        // "Main"이라는 이름의 씬으로 전환합니다.
        SceneManager.LoadScene("Main");
    }
}
