using UnityEngine;

public class ParticipantTracker : MonoBehaviour
{
    private Vector3 startPosition;
    private float movedDistance = 0f;

    public void ResetPosition(Vector3 position)
    {
        startPosition = position;
        movedDistance = 0f;
    }

    void Update()
    {
        // 实时追踪参与者移动
        movedDistance = Vector3.Distance(startPosition, Camera.main.transform.position);
    }

    public float GetMovedDistance()
    {
        return movedDistance;
    }
}
