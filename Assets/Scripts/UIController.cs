using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text playerKillsText;
    [SerializeField] Image staminaBarImage;
    [SerializeField] int playerKills = 0;
    [SerializeField] GameObject ListItemPrefab;
    [SerializeField] GameObject ImplantsList;
    [SerializeField] GameObject GameOverPanel;
    [SerializeField] TMP_Text GameOverText;

    public static UIController Instance;
    private List<GameObject> displayedImplants = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        GameOverPanel.SetActive(false);
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

    /// <summary>
    /// Update stamina bar.
    /// </summary>
    /// <param name="amount">Float from 0 to 1</param>
    public void UpdateStamina(float amount)
    {
        staminaBarImage.fillAmount = amount;
    }

    public void OnKill(DeathReason deathReason)
    {
        if (deathReason != DeathReason.PlayerAttack) return;

        playerKills += 1;
        UpdateKills();
    }

    public void OnPlayerMicroImplantsChange(List<MicroImplantData> microImplants)
    {
        // remove old list items
        foreach (GameObject item in displayedImplants) if (item != null) Destroy(item);
        displayedImplants.Clear();

        Dictionary<MicroImplantData, int> implantsCounts = new() {};

        foreach (MicroImplantData implant in microImplants)
        {
            if (implantsCounts.TryGetValue(implant, out var implantCount))
            {
                implantsCounts[implant] = implantCount + 1;
            }
            else
            {
                implantsCounts.Add(implant, 1);
            }
        }

        // create new
        foreach (MicroImplantData implant in implantsCounts.Keys)
        {
            if (implant.Name == "None") continue;
            GameObject implantListItem = Instantiate(ListItemPrefab, ImplantsList.transform);
            implantListItem.GetComponent<UIListItem>().Set(implant.Icon, implantsCounts[implant] + "x " + implant.Name);
            displayedImplants.Add(implantListItem);
        }

        Debug.Log("Micro-implants UIList was redrawn.");
    }

    public void OnPlayerDeath(DeathReason _)
    {
        GameOverText.text = "Game Over";
        GameOverPanel.SetActive(true);
    }

    public void OnPlayerWin()
    {
        GameOverText.text = "You win!";
        GameOverPanel.SetActive(true);
    }

    public void OnRestartButton()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        GameOverPanel.SetActive(false);
    }

    public void OnExitButton()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}