using UnityEngine;

public class ProgressCounter : MonoBehaviour
{
    public static int collectedCount = 0;

    public static void IncreaseCount()
    {
        collectedCount++;
        Debug.Log("Collected: " + collectedCount);
    }

    public static void ResetCount()
    {
        collectedCount = 0;
    }
}
