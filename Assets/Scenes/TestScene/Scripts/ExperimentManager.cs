using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExperimentManager : MonoBehaviour
{
    public GameObject spherePrefab; // 小球预制体
    public Transform startPoint;    // 起始点位置
    public TextMeshProUGUI instructionText; // 指令文本
    public Canvas blackScreen;      // 全黑画布
    public ParticipantTracker tracker;
    public DataLogger logger;

    private List<TrialCondition> conditions = new List<TrialCondition>();
    private int currentTrial = 0;
    private bool isWalkingPhase = false;
    private GameObject currentSphere;

    void Start()
    {
        GenerateTrialConditions();
        ShowInstruction();
    }

    void GenerateTrialConditions()
    {
        float[] distances = { 3f, 5f, 7f };
        float[] heights = { 0f, 0.5f, 1f };
        bool[] shadows = { true, false };

        foreach (var distance in distances)
        {
            foreach (var height in heights)
            {
                foreach (var shadow in shadows)
                {
                    conditions.Add(new TrialCondition(distance, height, shadow));
                }
            }
        }

        // 随机打乱条件顺序
        Shuffle(conditions);
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void ShowInstruction()
    {
        if (currentTrial >= conditions.Count)
        {
            instructionText.text = "感谢您的参与！按下扳机键退出测试。";
            return;
        }

        TrialCondition condition = conditions[currentTrial];
        instructionText.text = $"这是您的第 {currentTrial + 1}/18 次测试。\n" +
                               "观察小球10秒后进入盲走测试。\n" +
                               "按下扳机键开始。";
        instructionText.gameObject.SetActive(true);
        blackScreen.gameObject.SetActive(false);
        isWalkingPhase = false;
    }

    public void OnTriggerPressed()
    {
        if (instructionText.gameObject.activeSelf)
        {
            StartTrial();
        }
        else if (isWalkingPhase)
        {
            EndWalkingPhase();
        }
    }

    void StartTrial()
    {
        instructionText.gameObject.SetActive(false);
        tracker.ResetPosition(startPoint.position);

        TrialCondition condition = conditions[currentTrial];
        Vector3 position = startPoint.position + new Vector3(0, condition.height, condition.distance);
        currentSphere = Instantiate(spherePrefab, position, Quaternion.identity);
        currentSphere.GetComponent<ShadowController>().SetShadow(condition.shadow);

        Invoke(nameof(StartWalkingPhase), 10f); // 10秒后进入盲走阶段
    }

    void StartWalkingPhase()
    {
        Destroy(currentSphere);
        blackScreen.gameObject.SetActive(true);
        isWalkingPhase = true;
        instructionText.text = "请走到小球的位置处。\n按下扳机键确认位置。";
        instructionText.gameObject.SetActive(true);
    }

    void EndWalkingPhase()
    {
        float movedDistance = tracker.GetMovedDistance();
        TrialCondition condition = conditions[currentTrial];
        logger.LogData(currentTrial + 1, condition.distance, condition.height, condition.shadow, movedDistance);

        currentTrial++;
        ShowInstruction();
    }
}

public class TrialCondition
{
    public float distance;
    public float height;
    public bool shadow;

    public TrialCondition(float distance, float height, bool shadow)
    {
        this.distance = distance;
        this.height = height;
        this.shadow = shadow;
    }
}
