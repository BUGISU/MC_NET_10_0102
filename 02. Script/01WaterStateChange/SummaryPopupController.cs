using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummaryPopupController : MonoBehaviour
{
    [Header("UI 참조")]
    public GameObject popupPanel;
    public TextMeshProUGUI contentText;
    public Button previousButton;
    public Button nextButton;
    [SerializeField] private Transform finishIcon; // 회전할 아이콘
    [Header("페이지 데이터")]
    public PopupPageData pageData;  // ScriptableObject를 Inspector에서 할당

    private int currentPage = 0;

    private void Start()
    {
        /*popupPanel.SetActive(false);*/
        nextButton.onClick.AddListener(ShowNextPage);
        previousButton.onClick.AddListener(ShowPreviousPage);
    }
    public void OpenPopup()
    {
        if (pageData == null || pageData.pages.Count == 0)
        {
            Debug.LogWarning("페이지 데이터가 없어서 팝업을 열지 않습니다.");
            return;
        }

        currentPage = 0;
        popupPanel.SetActive(true);

        // 실제로 켜졌다면 사운드 재생
        if (popupPanel.activeInHierarchy)
        {
            ShowPage(currentPage);
            SoundManager.instance.PlaySFX("Summary");
            StartCoroutine(QuizEndIcon_RotateObject());
        }
    }


    void ShowPage(int index)
    {   
        if (pageData != null && index >= 0 && index < pageData.pages.Count)
        {
            contentText.text = pageData.pages[index].pageText;
            previousButton.gameObject.SetActive(index > 0);
            nextButton.gameObject.SetActive(index < pageData.pages.Count - 1);
        }
    }

    void ShowNextPage()
    {            
        SoundManager.instance.PlaySFX("StepSound");
        if (pageData != null && currentPage < pageData.pages.Count - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    void ShowPreviousPage()
    { 
        SoundManager.instance.PlaySFX("StepSound");
        if (pageData != null && currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }
    IEnumerator QuizEndIcon_RotateObject()
    {
        float angle = 15f;   // 최대 회전 각도
        float speed = 2f;    // 좌우 흔드는 속도
        float t = 0f;

        while (true)
        {
            t += Time.deltaTime * speed;

            // -1 ~ 1 사이로 반복
            float sinValue = Mathf.Sin(t);

            // UI 이미지라면 Z축 기준으로 흔들기
            finishIcon.localRotation = Quaternion.Euler(0f, 0f, sinValue * angle);

            yield return null;
        }
    }

}