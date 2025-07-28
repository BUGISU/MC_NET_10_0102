using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
[System.Serializable]
public class Quiz
{
    public int answerNumber;
    public string question;
    public List<string> choice;
    public string correct_reaction;
    public string wrong_reaction;
}
public class QuizSettingController : MonoBehaviour
{
    [Header("Quiz 문제 및 정답 연결")] public List<Quiz> quizList = new List<Quiz>();

    [SerializeField] private GameObject QuizStart; //퀴즈 시작
    [SerializeField] private GameObject QuizGroup; //퀴즈 문제
    [SerializeField] private GameObject QuizEnd; //요약정리
    [SerializeField] private Transform quizEndIcon; // 회전할 아이콘
    [SerializeField] private float startSpeed = 1440f; // 시작 각속도 (초당 1440도 = 4바퀴/초)
    [SerializeField] private float deceleration = 720f; // 감속도 (초당 얼마씩 속도를 줄일지)

    private int currentQuiz = 0;
    [SerializeField] private TextMeshProUGUI QuestionText;
    [SerializeField] private TextMeshProUGUI HintText;
    [SerializeField] private TextMeshProUGUI QuestionNumText;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private Button nextButton;

// QuizSettingController 안 Start() 같은 곳에서 초기 세팅 예시
    void Awake()
    {
        quizList = new List<Quiz>
        {
            new Quiz
            {
                question = "다음 중 물이 기체가 되기 위해 필요한 조건으로 \n알맞은 것은?",
                choice = new List<string>
                {
                    "차가운 곳에 두기",
                    "바람을 막기",
                    "따뜻하게 가열하기",
                    "물속에 넣기",
                    "어두운 곳에 두기"
                },
                answerNumber = 2,
                correct_reaction = "정답! \n따뜻하게 해야 수증기로 바뀌어요.",
                wrong_reaction = "다시 생각해보세요. \n따뜻한 열이 필요해요."
            },
            new Quiz
            {
                question = "플라스틱 컵에 물을 넣어 냉동실에 넣었더니 컵이 깨졌습니다. \n왜 그럴까요?",
                choice = new List<string>
                {
                    "물이 무거워져서",
                    "물이 차가워져서",
                    "물이 얼면서 부피가 커져서",
                    "물이 끓어서",
                    "물이 증발해서"
                },
                answerNumber = 2,
                correct_reaction = "정답! \n얼면 부피가 커져서 컵이 깨져요.",
                wrong_reaction = "다시 생각해보세요. \n물이 얼면 부피가 커집니다."
            },
            new Quiz
            {
                question = "컵 A : 뚜껑을 덮고 냉동실에 넣은 물\n컵 B : 뚜껑을 열고 냉동실에 넣은 물\n\n둘 중 어떤 컵이 먼저 얼까요?",
                choice = new List<string>
                {
                    "A가 먼저 얼어요.",
                    "B가 먼저 얼어요.",
                    "둘 다 동시에 얼어요.",
                    "둘 다 안 얼어요.",
                    "알 수 없어요."
                },
                answerNumber = 2,
                correct_reaction = "정답! \n조건이 같다면 동시에 얼어요.",
                wrong_reaction = "다시 생각해보세요. \n뚜껑과 상관없이 온도가 같다면 같아요."
            },
            new Quiz
            {
                question =
                    "같은 양의 물을 얼린 얼음 두 개가 있습니다.\n한 개는 작은 통에, 한 개는 넓은 접시에 놓았습니다.\n\n어느 것이 더 빨리 녹을까요?",
                choice = new List<string>
                {
                    "작은 통에 있는 얼음",
                    "넓은 접시에 있는 얼음",
                    "둘 다 똑같이 녹음",
                    "얼지 않는다",
                    "알 수 없다"
                },
                answerNumber = 1,
                correct_reaction = "정답! \n표면적이 넓은 접시 쪽이 빨리 녹아요.",
                wrong_reaction = "다시 생각해보세요. \n표면적이 중요해요."
            },
            new Quiz
            {
                question = "냉장고에서 꺼낸 음료수병 겉에 물방울이 생겼습니다.\n이 물방울은 어디서 온 것일까요?",
                choice = new List<string>
                {
                    "병 속에서 흘러나온 물",
                    "공기 중의 수증기가 물로 변한 것",
                    "병이 녹아서 나온 물",
                    "물이 스스로 생겨난 것",
                    "병을 씻고 남은 물"
                },
                answerNumber = 1,
                correct_reaction = "정답! \n공기 중 수증기가 차가운 병에 닿아 물로 바뀌었어요.",
                wrong_reaction = "다시 생각해보세요. \n병 속이 아니라 공기 중 수증기예요."
            },
            new Quiz
            {
                question = "바닷물이 증발해서 구름이 되고, 다시 비가 되어 내립니다.\n이 과정을 무엇이라 하나요?",
                choice = new List<string>
                {
                    "물의 응고",
                    "물의 순환",
                    "물의 얼리기",
                    "물의 분해",
                    "물의 증발"
                },
                answerNumber = 1,
                correct_reaction = "정답! \n물은 순환하면서 계속 이동해요.",
                wrong_reaction = "다시 생각해보세요. \n전체 흐름을 가리키는 말이에요."
            },
            new Quiz
            {
                question = "같은 양의 물을 컵 A(높고 좁음)와 \n컵 B(낮고 넓음)에 넣었습니다.\n둘 중 어느 쪽이 더 무겁나요?",
                choice = new List<string>
                {
                    "A가 더 무겁다",
                    "B가 더 무겁다",
                    "둘 다 같다",
                    "A는 가볍고 B는 무겁다",
                    "알 수 없다"
                },
                answerNumber = 2,
                correct_reaction = "정답! \n같은 양이면 무게도 같아요.",
                wrong_reaction = "다시 생각해보세요. \n모양이 달라도 양이 같으면 무게도 같아요."
            },
            new Quiz
            {
                question = "다음 중 물의 상태 변화와 관련 없는 것은?",
                choice = new List<string>
                {
                    "얼음이 녹아 물이 되는 것",
                    "물이 끓어 수증기로 바뀌는 것",
                    "수증기가 차가운 곳에서 물방울로 되는 것",
                    "물이 흘러서 강으로 모이는 것",
                    "물이 얼어붙는 것"
                },
                answerNumber = 3,
                correct_reaction = "정답! \n강으로 모이는 것은 상태 변화가 아니에요.",
                wrong_reaction = "다시 생각해보세요. \n상태가 바뀌는 것인지 아닌지 구분해보세요."
            },
            new Quiz
            {
                question = "뜨거운 물과 찬 물 중 어느 쪽이 더 빨리 증발할까요?",
                choice = new List<string>
                {
                    "뜨거운 물",
                    "찬 물",
                    "둘 다 똑같다",
                    "얼음물",
                    "알 수 없다"
                },
                answerNumber = 0,
                correct_reaction = "정답! \n뜨거운 물이 더 빨리 증발해요.",
                wrong_reaction = "다시 생각해보세요. \n온도가 높은 쪽을 떠올려보세요."
            },
            new Quiz
            {
                question = "구름이 만들어지기 위해 꼭 필요한 것은 무엇일까요?",
                choice = new List<string>
                {
                    "공기 중 먼지와 수증기",
                    "바닷속 모래",
                    "강한 바람",
                    "눈과 얼음",
                    "어두운 하늘"
                },
                answerNumber = 0,
                correct_reaction = "정답! \n공기 중 먼지와 수증기가 모여야 구름이 생깁니다.",
                wrong_reaction = "다시 생각해보세요. \n구름을 만들 재료가 무엇인지 떠올려보세요."
            },
        };
    }

    private void Start()
    {
        SetQuestion(currentQuiz);
        nextButton.onClick.AddListener(OnClick_NextButton);
    }

    public void SetQuestion(int currentQuiz)
    {
        foreach (var btn in choiceButtons)
        {
            btn.onClick.RemoveAllListeners();
        }

        nextButton.interactable = false; //다음으로 가기 버튼 비활성화
        QuestionText.text = quizList[currentQuiz].question;
        QuestionNumText.text = $"{currentQuiz + 1}/{quizList.Count}";
        HintText.text = "";
        //선택지 텍스트 입력
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text
                = quizList[currentQuiz].choice[i];
            if (i.Equals(quizList[currentQuiz].answerNumber))
            {
                choiceButtons[i].onClick.AddListener(OnClick_CorrectReaction);
            }
            else
            {
                choiceButtons[i].onClick.AddListener(OnClick_WrongReaction);
            }
        }
    }

    public void OnClick_CorrectReaction()
    {
        SoundManager.instance.PlaySFX("Correct");
        HintText.text = quizList[currentQuiz].correct_reaction;
        nextButton.interactable = true;
    }

    public void OnClick_WrongReaction()
    {
        SoundManager.instance.PlaySFX("Click2");
        HintText.text = quizList[currentQuiz].wrong_reaction;
    }

    public void OnClick_QuizStart()
    {
        QuizStart.SetActive(false);
        QuizGroup.SetActive(true);
        SetQuestion(currentQuiz);
    }

    public void OnClick_QuizEndClose()
    {
        QuizEnd.SetActive(false);
        QuizStart.SetActive(true);
        currentQuiz = 0;
    }

    public void OnClick_NextButton()
    {
        currentQuiz++;
        if (currentQuiz >= quizList.Count)
        {
            // 문제 끝! 요약
            QuizEnd.SetActive(true);
            SoundManager.instance.PlaySFX("Bell Star 2");
            // 아이콘 회전 시작
            // StartCoroutine(QuizEndIcon_RotateObject());
            quizEndIcon.DORotate(new Vector3(0, 1440f, 0), 2f, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic); // 빠르게 돌다가 서서히 멈춤
        }
        else
        {
            SoundManager.instance.PlaySFX("NextQuiz");
            SetQuestion(currentQuiz);
        }
    }
    IEnumerator QuizEndIcon_RotateObject()
    {
        float currentSpeed = startSpeed;
        
        // 무한 루프처럼 돌다가 속도가 0이 되면 멈춤
        while (currentSpeed > 0f)
        {
            quizEndIcon.Rotate(Vector3.up, currentSpeed * Time.deltaTime);
        
            // 속도를 서서히 줄이기
            currentSpeed -= deceleration * Time.deltaTime;
        
            yield return null;
        }
        
        // 마지막에 각도를 딱 맞추고 싶다면 (가까운 0도로 스냅)
        Vector3 euler = quizEndIcon.eulerAngles;
        euler.y = Mathf.Round(euler.y / 45f) * 45f; // 45도 단위로 맞추기 (선택)
        quizEndIcon.eulerAngles = euler;
    }
}