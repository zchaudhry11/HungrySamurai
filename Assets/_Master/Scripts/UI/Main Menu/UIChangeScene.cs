using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class UIChangeScene : MonoBehaviour 
{
    public int sceneNum;

    #region AUDIO PARAMETERS
    public AudioClip activateSound;
    public float audioLength = 0.1f;
    private float audioTimer = 0;
    private bool clicked = false;
    #endregion

    #region DEPENDENCIES
    private AudioSource audioSource;
    #endregion

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (clicked)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Destroy(player);

            if (audioTimer > 0)
            {
                audioTimer -= Time.deltaTime;
            }
            else
            {
                SceneManager.LoadScene(sceneNum);
            }
        }
    }

    public void ChangeScene()
    {
        clicked = true;
        audioSource.PlayOneShot(activateSound);
        audioTimer = audioLength;
    }
}