using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExitButton : MonoBehaviour
{
    // 이 메서드를 버튼의 OnClick 이벤트에 연결하세요.
    public void ExitGame()
    {
#if UNITY_EDITOR
        // 에디터에서는 실행을 중단합니다.
        EditorApplication.isPlaying = false;
#else
            // 빌드된 게임에서는 애플리케이션을 종료합니다.
            Application.Quit();
#endif
    }
}
