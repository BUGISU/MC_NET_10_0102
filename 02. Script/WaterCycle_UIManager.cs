using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using LeiaUnity;

[System.Serializable]
public class Cycle
{
    public Button PopupOpenButton;
    public GameObject PopupObject;
    public GameObject LabelObject;
    public Button CloseButton;
    // Transform 대신 벡터값으로 직접 지정
    public Vector3 cameraTargetPosition;
    public Vector3 cameraTargetRotationEuler; // 카메라가 바라볼 회전도 벡터로
    public float cameraTargetLeiaFocalDistance; 
    public string soundClipName;
}

public class WaterCycle_UIManager : MonoBehaviour
{
    [Header("메인 카메라")]
    public Camera mainCamera;
    private float originalleiaFocalDistance; 
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;

    [Header("카메라 이동 속도(초)")]
    public float moveDuration = 1f;

    [Header("Cycle 관련")]
    public List<Cycle> cycleList = new List<Cycle>();
    //public GameObject WorldLabelObject;
    void Start()
    {
      
        foreach (var cycle in cycleList)
        {
            cycle.PopupOpenButton.onClick.AddListener(() =>
            {
                SoundManager.instance.PlaySFX("Click4");
                MoveCameraToCycle(cycle);
            });
            
            if (cycle.CloseButton != null)
            {
                cycle.CloseButton.onClick.AddListener(() =>
                {
                    ClosePopup(cycle);
                });
            }
        }
    }

    void MoveCameraToCycle(Cycle cycle)
    {
        // 현재 카메라 위치/회전 저장
        originalCamPos = mainCamera.transform.position;
        originalCamRot = mainCamera.transform.rotation;

        // LeiaDisplay 현재 초점 거리 저장
        LeiaDisplay leia = mainCamera.transform.GetComponentInChildren<LeiaDisplay>();
        originalleiaFocalDistance = leia.FocalDistance;

        // 목표 위치, 회전
        Vector3 targetPos = cycle.cameraTargetPosition;
        Quaternion targetRot = Quaternion.Euler(cycle.cameraTargetRotationEuler);

        // 다른 라벨 모두 비활성
        foreach (var c in cycleList)
        {
            if (c.LabelObject != null)
                c.LabelObject.SetActive(false);
        }

        // DOTween으로 카메라 이동/회전
        mainCamera.transform.DOMove(targetPos, moveDuration);
        mainCamera.transform.DORotateQuaternion(targetRot, moveDuration)
            .OnComplete(() =>
            {
                cycle.PopupObject.SetActive(true);
                // 🎵 팝업 전용 사운드 재생
                if (!string.IsNullOrEmpty(cycle.soundClipName))
                {
                    SoundManager.instance.PlayNature(cycle.soundClipName);
                }
            });

        // Leia FocalDistance 부드럽게 보간
        DOTween.To(
            () => leia.FocalDistance,
            x => leia.FocalDistance = x,
            cycle.cameraTargetLeiaFocalDistance,
            moveDuration
        );
    }


    public void ClosePopup(Cycle cycle)
    {
        SoundManager.instance.PlaySFX("StepSound");
        cycle.PopupObject.SetActive(false);
        // 🎵 팝업 전용 사운드 정지
        if (!string.IsNullOrEmpty(cycle.soundClipName))
        {
            SoundManager.instance.StopNature();
        }
        foreach (var c in cycleList)
        {
            if (c.LabelObject != null)
                c.LabelObject.SetActive(true);
        }

        LeiaDisplay leia = mainCamera.transform.GetComponentInChildren<LeiaDisplay>();

        mainCamera.transform.DOMove(originalCamPos, moveDuration);
        mainCamera.transform.DORotateQuaternion(originalCamRot, moveDuration);

        DOTween.To(
            () => leia.FocalDistance,
            x => leia.FocalDistance = x,
            originalleiaFocalDistance,
            moveDuration
        ).SetEase(Ease.InOutSine);
    }
}
