using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Input;

public class ExperimentManager : MonoBehaviour, IMixedRealityPointerHandler
{
    public GameObject spherePrefab; 
    public Transform startPoint;    
    public GameObject instructionCanvas; 
    public GameObject blackScreenCanvas; 
    public ParticipantTracker tracker;
    public DataLogger logger;
    public GameObject ShadowPrefab;
    public Transform Plane;
    public GameObject Reminder;

    private List<TrialCondition> conditions = new List<TrialCondition>();
    private int currentTrial = 0;
    private bool isWalkingPhase = false;
    private bool experimentStarted = false;
    private GameObject currentSphere;
    private GameObject shadowCreated;

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

        // Make object order random
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
            var instructionText = instructionCanvas.GetComponentInChildren<TextMeshPro>();
            instructionText.text = "Thank you for your participation! Trigger to finish the experiment";
            instructionCanvas.SetActive(true);
            blackScreenCanvas.SetActive(false);
            return;
        }

        var trial = conditions[currentTrial];
        var instructionTextComponent = instructionCanvas.GetComponentInChildren<TextMeshPro>();
        //instructionTextComponent.text = $"This is your {currentTrial + 1}/18 trial \n" +
                                         //"Observe 10s and then take a blind-walking \n" +
                                         //"Trigger to start";
        instructionCanvas.SetActive(true);
        blackScreenCanvas.SetActive(false);
        experimentStarted = false;
        isWalkingPhase = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (instructionCanvas.activeSelf && !experimentStarted)
            {
                experimentStarted = true;
                StartTrial();
            }
            else if (isWalkingPhase)
            {
                EndWalkingPhase();
            }
        }
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData) { }
    public void OnPointerDown(MixedRealityPointerEventData eventData) { }
    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }
    public void OnPointerUp(MixedRealityPointerEventData eventData) { }

    void StartTrial()
    {
        Reminder.SetActive(false);
        instructionCanvas.SetActive(false);
        blackScreenCanvas.SetActive(false);
        tracker.ResetPosition(startPoint.position);

        TrialCondition condition = conditions[currentTrial];
        Vector3 position = startPoint.position + new Vector3(0, condition.height, condition.distance);
        currentSphere = Instantiate(spherePrefab, position, Quaternion.identity);
        if (condition.shadow)
        {
            shadowCreated = Instantiate(ShadowPrefab);
            shadowCreated.transform.position = new Vector3(position.x, Plane.position.y + 0.05f, position.z);
            
        }
        //currentSphere.GetComponent<ShadowController>().SetShadow(condition.shadow);

        Invoke(nameof(StartWalkingPhase), 5f); // 5 seconds to see the sphere
    }

    void StartWalkingPhase()
    {
        Destroy(currentSphere);
        Destroy(shadowCreated);
        blackScreenCanvas.SetActive(true);
        isWalkingPhase = true;
        instructionCanvas.SetActive(false);
        experimentStarted = false;

        var instructionText = instructionCanvas.GetComponentInChildren<TextMeshPro>();
        //instructionText.text = "Please walk to position of sphere\n and trigger to finish your walking";
    }

    void EndWalkingPhase()
    {
        Reminder.SetActive(true);
        blackScreenCanvas.SetActive(false);
        float movedDistance = tracker.GetMovedDistance();
        TrialCondition condition = conditions[currentTrial];
        logger.LogData(currentTrial + 1, condition.distance, condition.height, condition.shadow, movedDistance);

        currentTrial++;
        Invoke(nameof(ShowInstruction), 1f); // wait 1s to avoid entering directly to next trial 
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