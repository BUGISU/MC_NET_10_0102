using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

[System.Serializable]
public class DescriptionMent
{
    public string description;
}

public class NarrationController : MonoBehaviour
{
    [Header("나레이션 UI")]
    public GameObject narrationPanel;            // 대화창 패널
    public TextMeshProUGUI narrationText;        // 출력할 TextMeshPro
    public float typingSpeed = 0.05f;            // 글자 출력 속도
    public float fadeDuration = 0.3f;            // 페이드 인/아웃 시간

    public List<DescriptionMent> descriptions = new List<DescriptionMent>();

    /// <summary> 현재 나레이션 재생 중인지 여부 </summary>
    public bool IsNarrationPlaying => currentCoroutine != null;

    private Coroutine currentCoroutine;
    private CanvasGroup canvasGroup;

    // 🔥 스킵 여부 플래그
    private bool skipRequested = false;

    void Awake()
    {
        // CanvasGroup 가져오기
        canvasGroup = narrationPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = narrationPanel.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        narrationPanel.SetActive(false);

        // 예시 데이터 (실제 사용시 외부에서 세팅 가능)
        descriptions = new List<DescriptionMent>
        {
            new DescriptionMent
            {
                description = "알코올램프의 열로 얼음이 점점 녹아 물로 변하고 있어요.\n이처럼 고체가 열을 받아 액체로 바뀌는 것을 <color=#00FF7F>융해</color>라고 해요.",
            },
            new DescriptionMent
            {
                description = "물이 계속 끓으면서 눈에 보이지 않는 수증기로 변하고 있어요.\n이처럼 액체가 열을 받아 기체로 바뀌는 것을 <color=#00FF7F>기화</color>라고 해요.",
            },
            new DescriptionMent
            {
                description = "차가운 냉동실에 들어간 비커 표면에 물방울이 맺히고 있어요.\n이처럼 기체가 식어서 액체로 바뀌는 것을 <color=#00FF7F>응결</color>이라고 해요.",
            },
            new DescriptionMent
            {
                description = "물이 냉장고 안에서 차가워지면서 다시 얼음이 되었어요.\n이처럼 액체가 식어서 고체로 바뀌는 것을 <color=#00FF7F>응고</color>라고 해요.",
            },
        };
    }
    void Start()
    {
        // narrationPanel에 Button 컴포넌트가 붙어있다고 가정
        var btn = narrationPanel.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() =>
            {
                SkipNarration();
            });
        }
    }
    /// <summary>
    /// 외부에서 인덱스로 호출
    /// </summary>
    public void ShowNarrationByIndex(int index, float showTime = 2f)
    {
        if (index < 0 || index >= descriptions.Count)
        {
            Debug.LogWarning("인덱스 범위를 벗어났습니다.");
            return;
        }
        ShowNarration(descriptions[index].description, showTime);
    }

    /// <summary>
    /// 외부에서 바로 문자열로 호출
    /// </summary>
    public void ShowNarration(string message, float showTime = 2f)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        currentCoroutine = StartCoroutine(NarrationRoutine(message, showTime));
    }

    /// <summary>
    /// 외부에서 스킵 호출 (버튼에서 사용)
    /// </summary>
    public void SkipNarration()
    {
        if (IsNarrationPlaying)
        {
            skipRequested = true;
        }
    }

    private IEnumerator NarrationRoutine(string message, float showTime)
    {
        narrationPanel.SetActive(true);
        narrationText.text = "";
        skipRequested = false;

        // DOTween으로 페이드 인
        canvasGroup.DOKill();
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, fadeDuration);

        // 한 글자씩 출력
        yield return StartCoroutine(TypeTextWithRichText(message));

        // showTime 동안 대기하되 스킵 요청 시 즉시 종료
        float timer = 0f;
        while (timer < showTime && !skipRequested)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // DOTween으로 페이드 아웃
        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            narrationPanel.SetActive(false);
        });

        currentCoroutine = null;
    }

    /// <summary>
    /// 리치텍스트 태그를 깨지지 않게 한글자씩 출력 (스킵 지원)
    /// </summary>
    private IEnumerator TypeTextWithRichText(string fullText)
    {
        narrationText.text = "";
        string currentVisible = "";
        bool insideTag = false;

        foreach (char c in fullText)
        {
            if (skipRequested)
            {
                // 스킵 시 전체 텍스트 출력 후 즉시 종료
                narrationText.text = fullText;
                yield break;
            }

            if (c == '<') insideTag = true;
            currentVisible += c;
            if (c == '>') insideTag = false;

            if (!insideTag)
            {
                narrationText.text = currentVisible;
                yield return new WaitForSeconds(typingSpeed);
            }
        }
    }
}
