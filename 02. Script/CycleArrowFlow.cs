using System.Collections;
using UnityEngine;

public class CycleArrowFlow : MonoBehaviour
{
    public GameObject[] arrowObjects; // 위→아래 순서로 넣기
    [SerializeField] private float interval = 0.3f;
    [SerializeField] private float onTime = 0.4f;

    private Coroutine flowCoroutine;

    void OnEnable()
    {
        // 부모가 다시 활성화되면 코루틴 재시작
        flowCoroutine = StartCoroutine(PlayArrowFlow());
    }

    void OnDisable()
    {
        // 부모가 비활성화되면 코루틴 중지
        if (flowCoroutine != null)
        {
            StopCoroutine(flowCoroutine);
            flowCoroutine = null;
        }

        // 혹시 꺼지지 않은 화살표가 남아있을 수 있으니 모두 끄기
        foreach (var arrow in arrowObjects)
        {
            if (arrow != null) arrow.SetActive(false);
        }
    }

    IEnumerator PlayArrowFlow()
    {
        while (true)
        {
            for (int i = 0; i < arrowObjects.Length; i++)
            {
                arrowObjects[i].SetActive(true);
                StartCoroutine(TurnOffAfterDelay(arrowObjects[i], onTime));
                yield return new WaitForSeconds(interval);
            }
        }
    }

    IEnumerator TurnOffAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
}