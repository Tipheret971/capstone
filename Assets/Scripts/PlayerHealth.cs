using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    private PlayerRespawner respawner;

    private void Start()
    {
        currentHealth = maxHealth;
        respawner = GetComponent<PlayerRespawner>();
        if (respawner == null)
        {
            Debug.LogError("PlayerRespawner가 필요합니다!");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("현재 체력: " + currentHealth);

        if (currentHealth <= 0)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        currentHealth = maxHealth;
        respawner.Respawn();
    }
}