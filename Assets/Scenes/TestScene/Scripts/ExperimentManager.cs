using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExperimentManager : MonoBehaviour
{
    public GameObject spherePrefab; // С��Ԥ����
    public Transform startPoint;    // ��ʼ��λ��
    public TextMeshProUGUI instructionText; // ָ���ı�
    public Canvas blackScreen;      // ȫ�ڻ���
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

        // �����������˳��
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
            instructionText.text = "��л���Ĳ��룡���°�����˳����ԡ�";
            return;
        }

        TrialCondition condition = conditions[currentTrial];
        instructionText.text = $"�������ĵ� {currentTrial + 1}/18 �β��ԡ�\n" +
                               "�۲�С��10������ä�߲��ԡ�\n" +
                               "���°������ʼ��";
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

        Invoke(nameof(StartWalkingPhase), 10f); // 10������ä�߽׶�
    }

    void StartWalkingPhase()
    {
        Destroy(currentSphere);
        blackScreen.gameObject.SetActive(true);
        isWalkingPhase = true;
        instructionText.text = "���ߵ�С���λ�ô���\n���°����ȷ��λ�á�";
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
