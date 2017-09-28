using UnityEngine;
using UnityEngine.UI;

public class UIStatManager : MonoBehaviour 
{
    #region DEPENDENCIES
    private StatManager playerStats;
    #endregion

    // Objects associated with a character's life stats. All bars are followed by a FillerDecay object which is used to add a smooth loss effect when the value decreases.
    #region LIFE STAT ELEMENTS
    public Image pHealthBar;
    public FillerDecay pHealthDecay;
    #endregion

    private void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<StatManager>();
    }

    private void LateUpdate()
    {
        UpdatePlayerHealth();
    }

    public void UpdatePlayerHealth()
    {
        pHealthBar.fillAmount = ((float)playerStats.HP/playerStats.maxHP);
        pHealthDecay.DecayFill();
    }
}