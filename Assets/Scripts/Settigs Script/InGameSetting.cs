using UnityEngine;

public class InGameSetting : MonoBehaviour
{
    [Header("Settings Panel")]
    // 인게임 설정 패널(GameObject)을 Inspector에서 할당하세요.
    public GameObject settingsPanel;

    /// <summary>
    /// 설정 패널을 열면서 게임 일시정지 및 오디오 정지를 적용합니다.
    /// </summary>
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            Time.timeScale = 0f;        // 게임 시간 일시정지
            AudioListener.pause = true; // 오디오 정지
            Debug.Log("Settings open: Game paused and audio paused");
        }
        else
        {
            Debug.LogWarning("Settings panel is not assigned!");
        }
    }

    /// <summary>
    /// 설정 패널을 닫으면서 게임 재개 및 오디오 재생을 적용합니다.
    /// </summary>
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            Time.timeScale = 1f;         // 게임 재개
            AudioListener.pause = false; // 오디오 재생
            Debug.Log("Settings closed: Game resumed and audio resumed");
        }
        else
        {
            Debug.LogWarning("Settings panel is not assigned!");
        }
    }

    /// <summary>
    /// 설정 패널의 활성 상태를 토글하며 적절히 게임 일시정지 및 재개를 처리합니다.
    /// </summary>
    public void ToggleSettings()
    {
        if (settingsPanel != null)
        {
            bool isActive = settingsPanel.activeSelf;
            if (isActive)
                CloseSettings();
            else
                OpenSettings();
        }
        else
        {
            Debug.LogWarning("Settings panel is not assigned!");
        }
    }
}
