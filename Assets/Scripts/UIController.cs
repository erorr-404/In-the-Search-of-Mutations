using TMPro;
using UnityEngine;


public class UIController : MonoBehaviour
{
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text playerKillsText;
    [SerializeField] int playerKills = 0;

    public static UIController Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateHealth(HealthChange healthChange)
    {
        string newText = System.Math.Round(healthChange.NewHealth, 2).ToString() + "/" + healthChange.MaxHealth.ToString();
        healthText.text = newText;
    }

    public void UpdateKills()
    {
        playerKillsText.text = playerKills.ToString();
    }

    public void OnKill(DeathReason deathReason)
    {
        if (deathReason != DeathReason.PlayerAttack) return;

        playerKills += 1;
        UpdateKills();
    }
}