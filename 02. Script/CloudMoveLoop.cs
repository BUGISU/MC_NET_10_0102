using UnityEngine;
using DG.Tweening;

public class CloudMoveLoop : MonoBehaviour
{
    public Transform startTarget; // Cloud_Polygon_Blender_2
    public Transform endTarget;   // Cloud_Polygon_Blender_1
    public float moveDuration = 3f;  // 이동 시간
    public float waitTime = 2f;      // 도착 후 대기 시간

    void Start()
    {
        // 처음 위치를 start로
        transform.position = startTarget.position;

        // DOTween 시퀀스 사용
        Sequence seq = DOTween.Sequence();

        // 1. start → end 이동
        seq.Append(transform.DOMove(endTarget.position, moveDuration).SetEase(Ease.Linear));

        // 2. 도착 후 waitTime 동안 대기
        seq.AppendInterval(waitTime);

        // 3. 즉시 위치를 start로 리셋
        seq.AppendCallback(() =>
        {
            transform.position = startTarget.position;
        });

        // 4. 다시 이동하도록 반복
        seq.SetLoops(-1, LoopType.Restart); // 무한 반복
    }
}