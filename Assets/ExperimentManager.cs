using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Input;

public class ExperimentManager : MonoBehaviour, IMixedRealityPointerHandler
{
    public GameObject spherePrefab; // Sphere prefab
    public Transform startPoint;    // Starting position for the sphere
    public GameObject instructionCanvas; // Instruction canvas
    public GameObject blackScreenCanvas; // Black screen canvas
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

        // Shuffle the conditions to randomize trial order
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
        if (instructionCanvas == null)
        {
            Debug.LogError("instructionCanvas is null");
        }

        var instructionText = instructionCanvas.GetComponentInChildren<TextMeshPro>();
        if (instructionText == null)
        {
            Debug.LogError("TextMeshPro component is missing from instructionCanvas or its children");
        }
        instructionText.text = "Thank you for your participation! All trials completed.";
        instructionCanvas.SetActive(true);
        blackScreenCanvas.SetActive(false);
        return;
    }

    var trial = conditions[currentTrial];
    var instructionTextComponent = instructionCanvas.GetComponentInChildren<TextMeshPro>();
    if (instructionTextComponent == null)
    {
        Debug.LogError("TextMeshPro component is missing from instructionCanvas or its children");
    }
    instructionTextComponent.text = $"Trial {currentTrial + 1}/18\n" +
                                     "Observe the sphere for 10 seconds.\n" +
                                     "Press the trigger to begin.";
    instructionCanvas.SetActive(true);
    blackScreenCanvas.SetActive(false);
    isWalkingPhase = false;
}


    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (instructionCanvas.activeSelf)
        {
            StartTrial();
        }
        else if (isWalkingPhase)
        {
            EndWalkingPhase();
        }
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData) { }
    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }
    public void OnPointerUp(MixedRealityPointerEventData eventData) { }

    void StartTrial()
    {
        instructionCanvas.SetActive(false);
        tracker.ResetPosition(startPoint.position);

        // Get the current trial condition
        TrialCondition condition = conditions[currentTrial];
        
        // Set the sphere's position based on the condition
        Vector3 position = startPoint.position + new Vector3(0, condition.height, condition.distance);
        currentSphere = Instantiate(spherePrefab, position, Quaternion.identity);

        // Set the shadow of the sphere based on the condition
        currentSphere.GetComponent<ShadowController>().SetShadow(condition.shadow);

        // After 10 seconds, transition to the walking phase
        Invoke(nameof(StartWalkingPhase), 10f);
    }

    void StartWalkingPhase()
    {
        Destroy(currentSphere);
        blackScreenCanvas.SetActive(true);
        isWalkingPhase = true;

        var instructionText = instructionCanvas.GetComponentInChildren<TextMeshPro>();
        instructionText.text = "Walk to the estimated position of the sphere.\nPress the trigger when you are done.";
        instructionCanvas.SetActive(true);
    }

    void EndWalkingPhase()
    {
        float movedDistance = tracker.GetMovedDistance();
        TrialCondition condition = conditions[currentTrial];
        logger.LogData(currentTrial + 1, condition.distance, condition.height, condition.shadow, movedDistance);

        // Proceed to the next trial
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
