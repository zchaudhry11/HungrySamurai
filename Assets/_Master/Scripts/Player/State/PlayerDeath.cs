using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDeath : MonoBehaviour 
{
    #region DEPENDENCIES
    private StatManager playerStats;
    private GameObject playerUI;
    private GameObject gameOverUI;
    #endregion

    #region DEATH PARAMETERS
    public float restartTime = 3.0f;
    #endregion

    private void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<StatManager>();
        playerUI = GameObject.FindGameObjectWithTag("Player_UI");
        gameOverUI = playerUI.transform.GetChild(2).gameObject;
    }

    private void Update()
    {
        if (playerStats.HP <= 0)
        {
            //playerStats.gameObject.SetActive(false); // Disable player object

            if (gameOverUI)
            {
                gameOverUI.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReloadScene(0);
            }

        }
    }

    /// <summary>
    /// Reloads the scene.
    /// </summary>
    /// <param name="sceneNum">The integer representing the scene to reload.</param>
    private void ReloadScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }
}