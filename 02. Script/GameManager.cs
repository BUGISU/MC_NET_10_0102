using UnityEngine;
using UnityEngine.SceneManagement;
using LeiaUnity;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitSettings();
    }

    /// <summary>
    /// PlayerPrefs 값을 LeiaDisplay와 SoundManager에 적용
    /// </summary>
    private void InitSettings()
    {
        // LeiaDisplay 초기화
        LeiaDisplay leia = FindObjectOfType<LeiaDisplay>();
        if (leia != null)
        {
            float savedDepth = PlayerPrefs.GetFloat("DepthFactor", leia.DepthFactor);
            float savedLookAround = PlayerPrefs.GetFloat("LookAroundFactor", leia.LookAroundFactor);
            int saved3DMode = PlayerPrefs.GetInt("User3DMode", 1);

            leia.DepthFactor = savedDepth;
            leia.LookAroundFactor = savedLookAround;
            leia.Set3DMode(saved3DMode == 1);
        }

        // SoundManager 초기화
        SoundManager sound = FindObjectOfType<SoundManager>();
        if (sound != null)
        {
            float savedBgm = PlayerPrefs.GetFloat("BGM_VOLUME", sound.bgmVolume);
            float savedSfx = PlayerPrefs.GetFloat("SFX_VOLUME", sound.sfxVolume);

            sound.bgmVolume = savedBgm;
            sound.sfxVolume = savedSfx;
        }

      //  Debug.Log($"[GameManager] {SceneManager.GetActiveScene().name} 씬에서 설정 적용 완료!");
    }
    public void LoadSceneName(string sceneName)
    {
        SoundManager.instance.PlaySFX("Back");
        SceneManager.LoadScene(sceneName);
    }
    
    public void PopupActive(GameObject popoUp)
    {     
        SoundManager.instance.PlaySFX("Back");
        bool selfState = popoUp.activeSelf;
        popoUp.SetActive(!selfState);
    }
}