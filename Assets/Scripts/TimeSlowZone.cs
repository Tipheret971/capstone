using UnityEngine;

public class TimeSlowZone : MonoBehaviour
{
    public float slowTimeScale = 0.3f;
    public AudioSource bgm;

    private bool isPlayerInside = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPlayerInside)
        {
            isPlayerInside = true;
            Time.timeScale = slowTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            if (bgm != null)
                bgm.pitch = slowTimeScale;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isPlayerInside)
        {
            isPlayerInside = false;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;

            if (bgm != null)
                bgm.pitch = 1f;
        }
    }
}
