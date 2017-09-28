using UnityEngine;
using UnityEngine.UI;

public class CoinUI : MonoBehaviour
{
    #region DEPENDENCIES
    private StatManager playerStats;
    private Text coinText;
    #endregion

    private void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<StatManager>();
        coinText = this.GetComponent<Text>();
    }

    private void FixedUpdate()
    {
        coinText.text = playerStats.money.ToString();
    }
}