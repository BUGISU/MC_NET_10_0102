using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MassVolumeController : MonoBehaviour
{
    [Header("오브젝트 참조")] public GameObject waterObject;
    public GameObject vaporEffect;
    public Material[] waterMaterial; //0:물, 1:얼음

    [Header("UI 참조")] public TextMeshProUGUI infoText;
    public TextMeshProUGUI weightText;
    public Button meltButton;
    public Button boilButton;
    public Button compareButton;
    public Button resetButton;

    [Header("팝업")] public GameObject Popup_WeightVolume;
    public TextMeshProUGUI weightPopupTitle;
    public TextMeshProUGUI weightPopupText;

    public TextMeshProUGUI volumePopupTitle;
    public TextMeshProUGUI volumePopupText;

    private bool _weightvolumePopupActive = false;

    [Header("설정")] public float weight = 100f; // g

    private enum State
    {
        Ice,
        Water,
        Vapor
    }

    private State currentState = State.Ice;

    // 초기 스케일을 저장할 변수
    private Vector3 initialScale;

    void Start()
    {
        // 시작할 때 waterObject의 현재 스케일을 저장
        initialScale = waterObject.transform.localScale;

        // 초기 상태 세팅
        SetState(State.Ice);

        meltButton.onClick.AddListener(OnMelt);
        boilButton.onClick.AddListener(OnBoil);
        compareButton.onClick.AddListener(OnClickWeightVolumePopup);
        resetButton.onClick.AddListener(OnReset);
    }

    void SetState(State newState)
    {
        currentState = newState;

        waterObject.SetActive(false);
        vaporEffect.SetActive(false);

        // 기본적으로 다 비활성화한 뒤 필요한 것만 켜는 방식
        meltButton.interactable = false;
        boilButton.interactable = false;
        resetButton.interactable = true; // 초기화는 항상 가능하도록

        switch (currentState)
        {
            case State.Ice:
                waterObject.SetActive(true);
                waterObject.GetComponent<Renderer>().material = waterMaterial[1];
                waterObject.transform.localScale = initialScale;
                infoText.text = "지금은 단단한 얼음 상태예요.";
                weightText.text = $"{weight:F0} g";
                PopupSetCompareText();
                // 얼음 상태에서는 "얼음 녹이기"만 활성화
                meltButton.interactable = true;
                break;

            case State.Water:
                waterObject.SetActive(true);
                waterObject.GetComponent<Renderer>().material = waterMaterial[0];
                AnimateScaleY(initialScale.y * 0.8f);
                infoText.text = "얼음이 물로 변했어요!";
                weightText.text = $"{weight:F0} g";
                PopupSetCompareText();
                // 물 상태에서는 "물 끓이기"만 활성화
                boilButton.interactable = true;
                break;

            case State.Vapor:
                vaporEffect.SetActive(true);
                infoText.text = "물이 수증기로 변했어요!";
                weightText.text = $"{weight:F0} g";
                PopupSetCompareText();
                // 수증기 상태에서는 다음 버튼이 없으니 melt/boil은 비활성화
                break;
        }
    }

    public void OnMelt()
    {
        SoundManager.instance.PlaySFX("Click3");
        if (currentState == State.Ice) SetState(State.Water);
    }

    public void OnBoil()
    {
        SoundManager.instance.PlaySFX("Click3");
        if (currentState == State.Water) SetState(State.Vapor);
    }

    public void OnReset()
    {
        SoundManager.instance.PlaySFX("Click3");
        SetState(State.Ice);
    }

    // Y축만 애니메이션으로 조절
    void AnimateScaleY(float targetY)
    {
        // X,Z는 유지하고 Y만 변경
        Vector3 current = waterObject.transform.localScale;

        // DOTween으로 Y축 스케일만 부드럽게 변경
        waterObject.transform.DOScaleY(targetY, 1.0f)
            .SetEase(Ease.OutSine);
    }

    // 현재 상태 이름 반환 (팝업용)
    string GetStateName()
    {
        switch (currentState)
        {
            case State.Ice: return "얼음";
            case State.Water: return "물";
            case State.Vapor: return "수증기";
            default: return "알 수 없음";
        }
    }


    // 👉 무게 재기/부피 비교 버튼에 연결
    public void OnClickWeightVolumePopup()
    {
        SoundManager.instance.PlaySFX("Click3");
        _weightvolumePopupActive = !_weightvolumePopupActive;
        if (_weightvolumePopupActive)
        {
            Popup_WeightVolume.SetActive(true);
            PopupSetCompareText();
        }
        else
        {
            Popup_WeightVolume.SetActive(false);
        }
    }

    public void PopupSetCompareText()
    {
        weightPopupTitle.text = "<color=#FFD700>무게 비교</color>";
        volumePopupTitle.text = "<color=#1E90FF>부피 비교</color>";

        switch (currentState)
        {
            // ----------------- 얼음 -----------------
            case State.Ice:
                weightPopupText.text =
                    "<b><color=#00BFFF>현재 상태: 얼음</color></b>\n" +
                    $"<color=#FFFFFF>이 물질의 무게는 <b>{weight:F0}g</b> 입니다.</color>\n\n" +
                    "<color=#FFFFFF>아직은 단단한 얼음 상태로, <b>상태가 변하더라도 물질의 총량은 변하지 않기 때문에 무게는 언제나 같습니다.</b>\n" +
                    "따뜻한 곳에 두어 얼음이 녹아도, 다시 얼려도, 심지어 끓여서 수증기가 되어도 <b>무게는 그대로 유지</b>돼요.</color>";

                volumePopupText.text =
                    "<b><color=#00BFFF>현재 상태: 얼음</color></b>\n" +
                    "<color=#FFFFFF>얼음 속의 물 분자들은 <b>규칙적으로 단단히 배열</b>되어 있지만, 그 사이사이에 <b>빈틈</b>이 많이 존재합니다.\n" +
                    "이 때문에 같은 무게를 가진 물질이라도 <b><color=#FFA500>얼음 상태일 때는 부피가 가장 큽니다.</color></b>\n\n" +
                    "컵에 가득 담긴 얼음이 녹으면 물은 더 적은 공간을 차지하게 되지요.</color>";
                break;

            // ----------------- 물 -----------------
            case State.Water:
                weightPopupText.text =
                    "<b><color=#00BFFF>현재 상태: 물</color></b>\n" +
                    $"<color=#FFFFFF>이 물질의 무게는 <b>{weight:F0}g</b> 입니다.</color>\n\n" +
                    "<color=#FFFFFF><b>물로 변했지만 무게는 변하지 않습니다.</b>\n" +
                    "얼음에서 물로 상태가 바뀌어도 물질의 양 자체는 줄거나 늘지 않기 때문이에요.\n" +
                    "다만, 부피와 모양은 달라질 수 있답니다.</color>";

                volumePopupText.text =
                    "<b><color=#00BFFF>현재 상태: 물</color></b>\n" +
                    "<color=#FFFFFF>물 분자들은 서로를 느슨하게 붙잡고 자유롭게 움직여요.\n" +
                    "이 과정에서 <b>분자 사이의 빈틈이 줄어들어</b> 같은 무게를 차지하는 공간이 줄어듭니다.\n\n" +
                    "그래서 <b><color=#FFA500>물 상태일 때는 부피가 얼음보다 작습니다.</color></b></color>";
                break;

            // ----------------- 수증기 -----------------
            case State.Vapor:
                weightPopupText.text =
                    "<b><color=#00BFFF>현재 상태: 수증기</color></b>\n" +
                    $"<color=#FFFFFF>이 물질의 무게는 <b>{weight:F0}g</b> 입니다.</color>\n\n" +
                    "<color=#FFFFFF><b>물이 수증기로 변해도 무게는 변하지 않습니다.</b>\n" +
                    "물질의 총량이 줄어드는 것이 아니라, 단지 <b>분자들이 흩어져서 눈에 보이지 않는 기체 상태로</b> 바뀐 것뿐이에요.</color>";

                volumePopupText.text =
                    "<b><color=#00BFFF>현재 상태: 수증기</color></b>\n" +
                    "<color=#FFFFFF>물이 끓어 수증기가 되면, 물 분자들은 <b>열을 받아 아주 빠르게 움직이고</b> 서로 붙잡지 못한 채 <b>멀리멀리 흩어지게 됩니다.</b>\n" +
                    "이 때문에 같은 무게의 물질이라도 <b><color=#FFA500>수증기 상태에서는 부피가 매우 커집니다.</color></b>\n\n" +
                    "예를 들어, 물 한 컵이 모두 기체로 변하면 작은 방 안을 가득 채울 수 있을 정도로 부피가 커져요.</color>";
                break;
        }
    }
}