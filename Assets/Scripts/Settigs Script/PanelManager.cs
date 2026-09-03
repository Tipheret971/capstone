using UnityEngine;

public class PanelManager : MonoBehaviour
{
    // InGameSetting 스크립트를 Inspector에서 할당합니다.
    public InGameSetting inGameSetting;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inGameSetting != null && inGameSetting.settingsPanel != null)
            {
                // 설정 패널이 활성화되어 있으면 닫고, 아니면 엽니다.
                if (inGameSetting.settingsPanel.activeSelf)
                    inGameSetting.CloseSettings();
                else
                    inGameSetting.OpenSettings();
            }
            else
            {
                Debug.LogWarning("InGameSetting 또는 settingsPanel이 할당되지 않았습니다.");
            }
        }
    }
}
