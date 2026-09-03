using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SoundSettings : MonoBehaviour
{
    public AudioMixer audioMixer;          // AudioMixer 인스턴스
    public Slider bgmSlider;              // BGM 슬라이더
    public Slider seSlider;               // SE 슬라이더
    public TextMeshProUGUI bgmText;       // BGM 텍스트
    public TextMeshProUGUI seText;        // SE 텍스트

    void Start()
    {
        // 저장된 볼륨 값으로 초기 설정
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 100);
        seSlider.value = PlayerPrefs.GetFloat("SEVolume", 100);

        // 슬라이더 값에 맞춰 텍스트와 오디오 믹서 볼륨 반영
        ApplyVolume();

        // 슬라이더 값 변화에 따른 반응 함수 등록
        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        seSlider.onValueChanged.AddListener(OnSEVolumeChanged);
    }

    public void OnBGMVolumeChanged(float value)
    {
        // 슬라이더 값에 맞춰 오디오 믹서 볼륨 적용
        SetVolume("BGM", value);
        // 텍스트에 볼륨 숫자 실시간 표시
        bgmText.text = Mathf.Round(value) + "%";
        // PlayerPrefs에 저장
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void OnSEVolumeChanged(float value)
    {
        // 슬라이더 값에 맞춰 오디오 믹서 볼륨 적용
        SetVolume("SE", value);
        // 텍스트에 볼륨 숫자 실시간 표시
        seText.text = Mathf.Round(value) + "%";
        // PlayerPrefs에 저장
        PlayerPrefs.SetFloat("SEVolume", value);
    }

    void SetVolume(string channel, float value)
    {
        // Volume 값을 decibel로 변환 (Log10 공식 사용)
        float volume = Mathf.Log10(Mathf.Max(value, 0.0001f) / 100f) * 20f;
        audioMixer.SetFloat(channel, volume);
    }

    void ApplyVolume()
    {
        OnBGMVolumeChanged(bgmSlider.value);
        OnSEVolumeChanged(seSlider.value);
    }
}
