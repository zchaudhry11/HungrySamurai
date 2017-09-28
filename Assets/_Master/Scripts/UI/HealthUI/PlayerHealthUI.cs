using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    private Text healthText;
    private StatManager playerStats;

    private void Start()
    {
        healthText = this.GetComponent<Text>();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<StatManager>();
    }

    private void Update()
    {
        int currHP = playerStats.HP;

        if (currHP < 0)
        {
            currHP = 0;
        }

        healthText.text = currHP + "/" + playerStats.maxHP;
    }
}