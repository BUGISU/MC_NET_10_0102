using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePopupController : MonoBehaviour
{
    [SerializeField]private GameObject settingPopup;
    
    [SerializeField]private Button _mainButton; //메인으로
    [SerializeField]private Button _continueButton; //이어하기
    [SerializeField]private Button _restartButton; //다시시작
    [SerializeField]private Button _settingButton; //환경설정
    [SerializeField]private Button _exitButton; //종료하기

    private void Awake()
    {
        _mainButton.onClick.AddListener(OnClick_MainButton);
        _continueButton.onClick.AddListener(OnClick_ContinueButton);
        _restartButton.onClick.AddListener(OnClick_RestartButton);
        _settingButton.onClick.AddListener(OnClick_SettingButton);
        _exitButton.onClick.AddListener(OnClickExitButton);
        if(settingPopup == null)
            settingPopup = GameObject.Find("SettingPopup");
    }

    private void OnClick_MainButton()
    {
        SoundManager.instance.PlaySFX("Back");
        SceneManager.LoadScene(StringKeys.INTRO_SCENE);
    }
    private void OnClick_ContinueButton()
    {        
        SoundManager.instance.PlaySFX("Back");
        this.gameObject.SetActive(false);
    }
    private void OnClick_RestartButton()
    {        
        SoundManager.instance.PlaySFX("Back");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }  
    private void OnClick_SettingButton()
    {
        SoundManager.instance.PlaySFX("Back");
        settingPopup.SetActive(true);
    } 
    void OnClickExitButton()
    {
        SoundManager.instance.PlaySFX("Back");
        Application.Quit();
    }
}
