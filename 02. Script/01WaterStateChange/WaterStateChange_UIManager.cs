using UnityEngine;
using UnityEngine.UI;

public enum WaterStateStep
{
    SolidToLiquid,
    LiquidToGas,
    GasToLiquid,
    LiquidToSolid
}

public class WaterStateChange_UIManager : MonoBehaviour
{
    [Header("▶ 실험 컨트롤러 참조")] public ExperimentalController experimentalController;
    [Header("▶ 학습 내용 정리 팝업")] public SummaryPopupController summaryPopupController;

    [Header("▶ UI Buttons")] [SerializeField]
    private Button _btnSolidToLiquid;

    [SerializeField] private Button _btnLiquidToGas;
    [SerializeField] private Button _btnGasToLiquid;
    [SerializeField] private Button _btnLiquidToSolid;
    [SerializeField] private Button _btnSummary;

    private void Awake()
    {
        // 각 버튼 클릭 시 ExperimentalController의 메서드 호출
        _btnSolidToLiquid.onClick.AddListener(() =>
        {
            SoundManager.instance.PlaySFX("StepSound");
            experimentalController.OnClickSolidToLiquid();
            _btnSolidToLiquid.interactable = false;
        });
        _btnLiquidToGas.onClick.AddListener(() =>
        {
            SoundManager.instance.PlaySFX("StepSound");
            experimentalController.OnClickLiquidToGas(); 
            _btnLiquidToGas.interactable = false;
        });
        _btnGasToLiquid.onClick.AddListener(() =>
        {
            SoundManager.instance.PlaySFX("StepSound");
            experimentalController.OnClickGasToLiquid();
            _btnGasToLiquid.interactable = false;
        });
        _btnLiquidToSolid.onClick.AddListener(() =>
        {
            SoundManager.instance.PlaySFX("StepSound");
            experimentalController.OnClickLiquidToSolid();
            _btnLiquidToSolid.interactable = false;
        });
        _btnSummary.onClick.AddListener(() =>
        {   
            summaryPopupController.OpenPopup();
        });
        experimentalController.OnStateChangeFinished += HandleStateChangeFinished;
    }

    private void Start()
    {
        // 초기 버튼 활성 상태
        SetStateChangeButtonsActive(true, false, false, false, false);
    }

    // ✅ 코루틴이 끝났을 때 실행될 메서드
    private void HandleStateChangeFinished(WaterStateStep step)
    {
        switch (step)
        {
            case WaterStateStep.SolidToLiquid:
                // 고체->액체 끝났으니 액체→기체 버튼 활성화
                SetStateChangeButtonsActive(false, true, false, false, false);
                break;

            case WaterStateStep.LiquidToGas:
                // 액체->기체 끝났으니 기체→액체 버튼 활성화
                SetStateChangeButtonsActive(false, false, true, false, false);
                break;

            case WaterStateStep.GasToLiquid:
                // 기체->액체 끝났으니 액체→고체 버튼 활성화
                SetStateChangeButtonsActive(false, false, false, true, false);
                break;

            case WaterStateStep.LiquidToSolid:
                // 액체->고체 끝났으니 재시작 버튼 활성화
                SetStateChangeButtonsActive(false, false, false, false, true);
                break;
        }
    }

    private void OnDestroy()
    {
        // ✅ 이벤트 해제(메모리 누수 방지)
        if (experimentalController != null)
            experimentalController.OnStateChangeFinished -= HandleStateChangeFinished;
    }

    // 버튼 활성/비활성 컨트롤
    private void SetStateChangeButtonsActive(bool solidToLiquid, bool liquidToGas, bool gasToLiquid, bool liquidToSolid,
        bool summary)
    {
        _btnSolidToLiquid.interactable = solidToLiquid;
        _btnLiquidToGas.interactable = liquidToGas;
        _btnGasToLiquid.interactable = gasToLiquid;
        _btnLiquidToSolid.interactable = liquidToSolid;
        _btnSummary.interactable = summary;
    }
}