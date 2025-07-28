using System.Collections;
using System.Collections.Generic;
using LeiaUnity;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private LeiaDisplay leiaDisplay;
    [SerializeField] private SoundManager soundManager;

    [Header("사운드 조절")]
    [SerializeField] private Slider BgmSlider;
    [SerializeField] private Slider SfxSlider;

    [Header("깊이감 / 룩어라운드")]
    [SerializeField] private Slider DepthFactorSlider;
    [SerializeField] private Slider LookAroundSlider;

    [Header("3D Mode Setting")]
    [SerializeField] private Toggle Mode3DToggle;

    [SerializeField] private Button CloseButton;
    [SerializeField] private Button ExitButton;

    private bool is3DModeOn = true;

    void Start()
    {
        if (leiaDisplay == null)
            leiaDisplay = FindObjectOfType<LeiaDisplay>();

        if (soundManager == null)
            soundManager = FindObjectOfType<SoundManager>();

        // 🔹 LeiaDisplay 초기값
        float savedDepth = PlayerPrefs.GetFloat("DepthFactor", leiaDisplay.DepthFactor);
        float savedLookAround = PlayerPrefs.GetFloat("LookAroundFactor", leiaDisplay.LookAroundFactor);
        int saved3DMode = PlayerPrefs.GetInt("User3DMode", 0);
        
        leiaDisplay.DepthFactor = savedDepth;
        leiaDisplay.LookAroundFactor = savedLookAround;
        is3DModeOn = saved3DMode == 1;
        leiaDisplay.Set3DMode(is3DModeOn);

        DepthFactorSlider.value = savedDepth;
        LookAroundSlider.value = savedLookAround;
        Mode3DToggle.isOn = is3DModeOn;

        // 🔹 SoundManager 초기값
        BgmSlider.value = PlayerPrefs.GetFloat("BGM_VOLUME", soundManager.bgmVolume);
        SfxSlider.value = PlayerPrefs.GetFloat("SFX_VOLUME", soundManager.sfxVolume);

        soundManager.bgmVolume = BgmSlider.value;
        soundManager.sfxVolume = SfxSlider.value;

        // 이벤트 등록
        DepthFactorSlider.onValueChanged.AddListener(delegate { SetDepth(); });
        LookAroundSlider.onValueChanged.AddListener(delegate { SetLookAround(); });
        Mode3DToggle.onValueChanged.AddListener(On3DToggleChanged);

        BgmSlider.onValueChanged.AddListener(delegate { SetBgmVolume(); });
        SfxSlider.onValueChanged.AddListener(delegate { SetSfxVolume(); });

        CloseButton.onClick.AddListener(ClosePopupButton);
        ExitButton.onClick.AddListener(OnClickExitButton);
    }

    void SetDepth()
    {
        leiaDisplay.DepthFactor = DepthFactorSlider.value;
        PlayerPrefs.SetFloat("DepthFactor", DepthFactorSlider.value);
        PlayerPrefs.Save();
    }

    void SetLookAround()
    {
        leiaDisplay.LookAroundFactor = LookAroundSlider.value;
        PlayerPrefs.SetFloat("LookAroundFactor", LookAroundSlider.value);
        PlayerPrefs.Save();
    }

    void On3DToggleChanged(bool isOn)
    {
        SoundManager.instance.PlaySFX("Back");
        is3DModeOn = isOn;
        leiaDisplay.Set3DMode(is3DModeOn);
        PlayerPrefs.SetInt("User3DMode", is3DModeOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    // 🎵 BGM 볼륨
    void SetBgmVolume()
    {
        if (soundManager != null)
        {
            soundManager.bgmVolume = BgmSlider.value;
            PlayerPrefs.SetFloat("BGM_VOLUME", BgmSlider.value);
            PlayerPrefs.Save();
        }
    }

    // 🎵 SFX 볼륨
    void SetSfxVolume()
    {
        if (soundManager != null)
        {
            soundManager.sfxVolume = SfxSlider.value;
            PlayerPrefs.SetFloat("SFX_VOLUME", SfxSlider.value);
            PlayerPrefs.Save();
        }
    }
    void ClosePopupButton()
    {
        SoundManager.instance.PlaySFX("Back");
        this.gameObject.SetActive(false);
    }
    void OnClickExitButton()
    {
        SoundManager.instance.PlaySFX("Back");
      Application.Quit();
    }
}
