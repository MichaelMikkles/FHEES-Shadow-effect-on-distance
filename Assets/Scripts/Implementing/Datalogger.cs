using System.IO;
using UnityEngine;

public class DataLogger : MonoBehaviour
{
    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "experiment_data.csv");
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "Trial,Distance,Height,Shadow,DistanceMoved\n");
        }
    }

    public void LogData(int trial, float distance, float height, bool shadow, float movedDistance)
    {
        string data = $"{trial},{distance},{height},{shadow},{movedDistance}\n";
        File.AppendAllText(filePath, data);
        Debug.Log($"Data logged: {data}");
    }
}
