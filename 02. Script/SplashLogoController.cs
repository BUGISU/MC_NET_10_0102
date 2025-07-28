using System;
using UnityEngine;
using DG.Tweening;

public class SplashLogoController : MonoBehaviour
{
    [Header("▶ 전체 배경/로고 그룹(CanvasGroup)")] [SerializeField]
    private CanvasGroup splitTitleCanvasGroup;

    [Header("▶ 로고 트랜스폼")] [SerializeField] private RectTransform titleLogoTransform;

    [Header("▶ 타이밍 설정")] private float scaleUpSize = 1f; // 커질 크기
    private float scaleUpDuration = 0.3f; // 커지는 시간
    private float holdDuration = 1f; // 유지 시간
    private float scaleDownDuration = 0.5f; // 스케일 줄어드는 시간
    private float fadeOutDuration = 1f; // 배경 페이드아웃 시간

    private void Awake()
    {
        splitTitleCanvasGroup.gameObject.SetActive(true);
    }

    private void Start()
    {
        if (splitTitleCanvasGroup == null || titleLogoTransform == null)
        {
            Debug.LogError("❌ CanvasGroup 또는 TitleLogoTransform이 할당되지 않았습니다.");
            return;
        }

        // 초기 상태
        splitTitleCanvasGroup.alpha = 1f;
        titleLogoTransform.localScale = Vector3.zero;
   
        PlayLogoAnimation();
    }

    private void PlayLogoAnimation()
    {
        // 시퀀스 생성
        Sequence seq = DOTween.Sequence();

        // 1) 로고 스케일 0 → 1
        seq.Append(
            titleLogoTransform.DOScale(scaleUpSize, scaleUpDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    SoundManager.instance.PlaySFX("SplashLogo");
                })
        );

        // 2) 유지 (holdDuration만큼 대기)
        seq.AppendInterval(holdDuration);

        // 3) 로고 스케일 다운 + 배경 페이드아웃을 동시에 진행
        seq.Append(
            titleLogoTransform.DOScale(0f, scaleDownDuration)
                .SetEase(Ease.InBack)
        );

        seq.Join(
            splitTitleCanvasGroup.DOFade(0f, fadeOutDuration)
                .SetEase(Ease.Linear)
        );
        // 4) 모두 끝나면 오브젝트 비활성화
        seq.OnComplete(() =>
        {
            splitTitleCanvasGroup.gameObject.SetActive(false);
        });
    }
}