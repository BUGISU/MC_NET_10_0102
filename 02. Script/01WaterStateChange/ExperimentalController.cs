using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ExperimentalController : MonoBehaviour
{
    public NarrationController narrationController;
    public event Action<WaterStateStep> OnStateChangeFinished;
    [Header("▶ 상태 변화 애니메이터")]
    [SerializeField] private Animator _stateChangeCtrl;

    [Header("▶ 실험 관련 오브젝트 및 파티클")]
    [SerializeField] private GameObject _lamp;         // 히터(램프)
    [SerializeField] private GameObject _fridge;       // 냉장고
    [SerializeField] private GameObject _experimental; // 실험 물체 (비커 등)

    // 냉장고 밖 원래 위치/회전/스케일
    private Vector3 freezOriginalPos = new Vector3(0.0006792545f, -0.3573422f, -4.49f);
    private Vector3 freezOriginalRot = new Vector3(-10.088f, 0f, 0f);
    private Vector3 freezOriginalScale = new Vector3(1, 1, 1);

    // 냉장고 안으로 들어갈 때 위치/회전/스케일
    private Vector3 freezPos = new Vector3(-0.098f, 0.195f, -2.399f);
    private Vector3 freezRot = new Vector3(-10.088f, 0f, 0f);
    private Vector3 freezScale = new Vector3(0.6528621f, 0.6528621f, 0.6528621f);

    [SerializeField] private ParticleSystem _heatParticle;   // 불꽃
    [SerializeField] private ParticleSystem _meltParticle;   // 녹는 이펙트
    [SerializeField] private ParticleSystem _bubbleParticle; // 끓는 이펙트
    [SerializeField] private ParticleSystem _gasParticle;    // 기체 이펙트
    [SerializeField] private GameObject _mapObject;          // 맵(냉장고 쪽으로 이동하는 오브젝트)

    private Coroutine _blendCoroutine;
    private bool _blendFinished = false;   // BlendTree 완료 플래그
    private bool _actionFinished = false; // 이동 애니메이션 완료 플래그

    private Coroutine _solidToLiquidCoroutine;
    private Coroutine _liquidToGasCoroutine;
    private Coroutine _gasToLiquidCoroutine;
    private Coroutine _liquidToSolidCoroutine;

    private float _moveDistance = 4f;
    private float _moveDuration = 2f;

    // ------------------- 고체 → 액체 -------------------
    public void OnClickSolidToLiquid()
    {
        if (_solidToLiquidCoroutine != null) StopCoroutine(_solidToLiquidCoroutine);
        _solidToLiquidCoroutine = StartCoroutine(PlaySolidToLiquidRoutine());
    }

    private IEnumerator PlaySolidToLiquidRoutine()
    {
        _lamp.SetActive(true);
        _heatParticle.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        _stateChangeCtrl.SetTrigger("Heat");
        _meltParticle.gameObject.SetActive(true);
        narrationController.ShowNarrationByIndex(0, 3f);
        yield return new WaitForSeconds(1f);

        SetAnimatorBlend(0.5f);
        yield return new WaitUntil(() => _blendFinished);

        yield return new WaitUntil(() => narrationController.IsNarrationPlaying == false);

        _blendFinished = false;
        SetParticleActive(false, false, false);
        OnStateChangeFinished?.Invoke(WaterStateStep.SolidToLiquid);
    }

    // ------------------- 액체 → 기체 -------------------
    public void OnClickLiquidToGas()
    {
        if (_liquidToGasCoroutine != null) StopCoroutine(_liquidToGasCoroutine);
        _liquidToGasCoroutine = StartCoroutine(PlayLiquidToGasRoutine());
    }

    private IEnumerator PlayLiquidToGasRoutine()
    {
        // 비커 닫기
        yield return StartCoroutine(CloseBeaker());

        _lamp.SetActive(true);
        _heatParticle.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        _bubbleParticle.gameObject.SetActive(true);
        _gasParticle.gameObject.SetActive(true);
           narrationController.ShowNarrationByIndex(1, 3f);
        yield return new WaitForSeconds(2f);
        SetAnimatorBlend(1f);
     
        yield return new WaitUntil(() => _blendFinished);

        _bubbleParticle.gameObject.SetActive(false);
        yield return new WaitUntil(() => narrationController.IsNarrationPlaying == false);
        _blendFinished = false;
        SetParticleActive(false, false, false);
        OnStateChangeFinished?.Invoke(WaterStateStep.LiquidToGas);
    }

    // 비커 뚜껑 닫는 모션
    private IEnumerator CloseBeaker()
    {
        float speed = 0.5f;
        var plateTransform = _experimental.transform.Find("Beaker/SolidtoLiquid/Plate");
        if (plateTransform == null) yield break;
        plateTransform.gameObject.SetActive(true);

        Vector3 startPos = plateTransform.localPosition;
        Vector3 endPos = new Vector3(0f, startPos.y, startPos.z);

        float time = 0f;
        while (time < speed)
        {
            time += Time.deltaTime;
            float t = time / speed;
            plateTransform.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        plateTransform.localPosition = endPos;
    }

    // ------------------- 기체 → 액체 -------------------
    public void OnClickGasToLiquid()
    {
        if (_gasToLiquidCoroutine != null) StopCoroutine(_gasToLiquidCoroutine);
        _gasToLiquidCoroutine = StartCoroutine(PlayGasToLiquidRoutine());
    }

    private IEnumerator PlayGasToLiquidRoutine()
    {
        _lamp.SetActive(false);
        _heatParticle.gameObject.SetActive(false);

        // 맵 이동
        yield return StartCoroutine(MoveCoolingZoonRoutine());
        narrationController.ShowNarrationByIndex(2, 3f);
        // 냉장고 애니메이션
        yield return StartCoroutine(FreezingGas());
        yield return new WaitUntil(() => narrationController.IsNarrationPlaying == false);
        _blendFinished = false;
        SetParticleActive(false, false, false);
        OnStateChangeFinished?.Invoke(WaterStateStep.GasToLiquid);
    }

    // ------------------- 액체 → 고체 -------------------
    public void OnClickLiquidToSolid()
    {
        if (_liquidToSolidCoroutine != null) StopCoroutine(_liquidToSolidCoroutine);
        _liquidToSolidCoroutine = StartCoroutine(PlayLiquidToSolidRoutine());
    }

    private IEnumerator PlayLiquidToSolidRoutine()
    {
        // 냉장고 애니메이션
        yield return StartCoroutine(FreezingWater());
        narrationController.ShowNarrationByIndex(3, 3f);
        yield return new WaitUntil(() => narrationController.IsNarrationPlaying == false);
        _blendFinished = false;
        SetParticleActive(false, false, false);
        OnStateChangeFinished?.Invoke(WaterStateStep.LiquidToSolid);
    }

    // ------------------- 파티클 컨트롤 -------------------
    private void SetParticleActive(bool heat, bool melt, bool bubble)
    {
        _heatParticle.gameObject.SetActive(heat);
        _meltParticle.gameObject.SetActive(melt);
        _bubbleParticle.gameObject.SetActive(bubble);
    }

    // ------------------- BlendTree 컨트롤 -------------------
    private void SetAnimatorBlend(float target)
    {
        if (_blendCoroutine != null) StopCoroutine(_blendCoroutine);
        _blendCoroutine = StartCoroutine(BlendToTarget(target));
    }

    private IEnumerator BlendToTarget(float target)
    {
        float current = _stateChangeCtrl.GetFloat("Blend");
        float duration = 1.5f;
        float time = 0f;

        bool logged = false;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float blend = Mathf.Lerp(current, target, t);
            _stateChangeCtrl.SetFloat("Blend", blend);

            if (!logged && Mathf.Abs(blend - target) < 0.01f)
            {
                _blendFinished = true;
                logged = true;
            }
            yield return null;
        }
        _stateChangeCtrl.SetFloat("Blend", target);
    }

    // ------------------- 냉장고 쪽으로 이동 -------------------
    private IEnumerator MoveCoolingZoonRoutine()
    {
        Vector3 startPos = _mapObject.transform.position;
        Vector3 endPos = startPos + new Vector3(-_moveDistance, 0, 0);

        float time = 0f;
        while (time < _moveDuration)
        {
            time += Time.deltaTime;
            float t = time / _moveDuration;
            _mapObject.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        _mapObject.transform.position = endPos;
        _actionFinished = true;
    }

    // ------------------- 냉장고 안에서 기체 → 액체 -------------------
    private IEnumerator FreezingGas()
    {
        _actionFinished = false;
        Transform _experimentalTransform = _experimental.transform;
        Transform _fridge_left = _fridge.transform.Find("Fridge_SBS_DoorL").transform;
        Vector3 baseAngle = _fridge_left.localEulerAngles;

        // 문 열기
        yield return _fridge_left.DOLocalRotate(baseAngle + new Vector3(0, 130f, 0f), 1.5f, RotateMode.Fast)
            .SetEase(Ease.OutCubic).WaitForCompletion();

        // 물체 안으로 이동
        Sequence moveIn = DOTween.Sequence();
        moveIn.Join(_experimentalTransform.DOMove(freezPos, 1f).SetEase(Ease.Linear));
        moveIn.Join(_experimentalTransform.DORotate(freezRot, 1f).SetEase(Ease.Linear));
        moveIn.Join(_experimentalTransform.DOScale(freezScale, 1f).SetEase(Ease.Linear));
        yield return moveIn.WaitForCompletion();

        // 문 닫기
        yield return _fridge_left.DOLocalRotate(baseAngle, 1.5f, RotateMode.Fast)
            .SetEase(Ease.OutCubic).WaitForCompletion();

        // 블렌드 재생
        SetAnimatorBlend(0.5f);
        yield return new WaitUntil(() => _blendFinished);
        _blendFinished = false;
        yield return new WaitForSeconds(0.5f);

        _gasParticle.gameObject.SetActive(false);

        // 문 열기
        yield return _fridge_left.DOLocalRotate(baseAngle + new Vector3(0, 130f, 0f), 1.5f, RotateMode.Fast)
            .SetEase(Ease.OutCubic).WaitForCompletion();

        // 원위치
        Sequence moveOut = DOTween.Sequence();
        moveOut.Join(_experimentalTransform.DOMove(freezOriginalPos, 1f).SetEase(Ease.Linear));
        moveOut.Join(_experimentalTransform.DORotate(freezOriginalRot, 1f).SetEase(Ease.Linear));
        moveOut.Join(_experimentalTransform.DOScale(freezOriginalScale, 1f).SetEase(Ease.Linear));
        yield return moveOut.WaitForCompletion();

        // 문 닫기
        yield return _fridge_left.DOLocalRotate(baseAngle, 1.5f, RotateMode.Fast)
            .SetEase(Ease.OutCubic).WaitForCompletion();
        _actionFinished = true;
    }

    // ------------------- 냉장고 안에서 액체 → 고체 -------------------
    private IEnumerator FreezingWater()
    {
        _actionFinished = false;
        Transform _experimentalTransform = _experimental.transform;
        Transform _fridge_left = _fridge.transform.Find("Fridge_SBS_DoorL").transform;
        Vector3 baseAngle = _fridge_left.localEulerAngles;

        // 문 열기
        yield return _fridge_left.DOLocalRotate(baseAngle + new Vector3(0, 130f, 0f), 1.5f, RotateMode.Fast)
            .SetEase(Ease.OutCubic).WaitForCompletion();

        // 물체 안으로 이동
        Sequence moveIn = DOTween.Sequence();
        moveIn.Join(_experimentalTransform.DOMove(freezPos, 1f).SetEase(Ease.Linear));
        moveIn.Join(_experimentalTransform.DORotate(freezRot, 1f).SetEase(Ease.Linear));
        moveIn.Join(_experimentalTransform.DOScale(freezScale, 1f).SetEase(Ease.Linear));
        yield return moveIn.WaitForCompletion();

        // 문 닫기
        yield return _fridge_left.DOLocalRotate(baseAngle, 1.5f, RotateMode.Fast)
            .SetEase(Ease.OutCubic).WaitForCompletion();

        // 상태 전환 (액체 오브젝트 OFF, 고체 오브젝트 ON)
        var WaterGameObject = _experimental.transform.Find("Beaker/SolidtoLiquid/liquid (1)").gameObject;
        var freezIceGameObject = _experimental.transform.Find("Beaker/IceCube2").gameObject;
        WaterGameObject.SetActive(false);
        freezIceGameObject.SetActive(true);

        // 문 열기
        yield return _fridge_left.DOLocalRotate(baseAngle + new Vector3(0, 130f, 0f), 1.5f, RotateMode.Fast)
            .SetEase(Ease.OutCubic).WaitForCompletion();

        // 원위치
        Sequence moveOut = DOTween.Sequence();
        moveOut.Join(_experimentalTransform.DOMove(freezOriginalPos, 1f).SetEase(Ease.Linear));
        moveOut.Join(_experimentalTransform.DORotate(freezOriginalRot, 1f).SetEase(Ease.Linear));
        moveOut.Join(_experimentalTransform.DOScale(freezOriginalScale, 1f).SetEase(Ease.Linear));
        yield return moveOut.WaitForCompletion();

        // 문 닫기
        yield return _fridge_left.DOLocalRotate(baseAngle, 1.5f, RotateMode.Fast)
            .SetEase(Ease.OutCubic).WaitForCompletion();
        _actionFinished = true;
    }
}
