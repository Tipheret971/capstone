using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SettingsOpener : MonoBehaviour
{
    [Header("UI")]
    public GameObject settingsPanel; // 설정 패널 (기본적으로 비활성화)

    private void Start()
    {
        // 초기화 시 별도의 게임 일시정지나 오디오 정지 처리는 하지 않습니다.
    }

    // 설정 패널의 활성 상태만 토글합니다.
    public void ToggleSettings()
    {
        bool isActive = settingsPanel.activeSelf;
        bool newState = !isActive;
        settingsPanel.SetActive(newState);
    }

    // 설정 패널을 닫습니다.
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
}
