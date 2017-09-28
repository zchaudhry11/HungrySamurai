using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HealingPickup : MonoBehaviour, IPickup
{
    #region DEPENDENCIES
    private StatManager playerStats;
    private AudioSource audioSource;
    private Renderer rend;
    #endregion

    #region STATE
    private bool pickedUp = false;
    #endregion

    #region HEAL PARAMETERS
    public int restoreAmount = 25;
    #endregion

    #region PICKUP EFFECTS
    public AudioClip pickupSound;
    public float destroyDelay = 0.60f; // Seconds to wait before destroying a de-activated object

    // particle system
    #endregion

    private void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<StatManager>();
        audioSource = this.GetComponent<AudioSource>();
        rend = this.GetComponent<Renderer>();
        destroyDelay += Random.Range(0.0f, 0.6f); // Add a random offset to reduce chance of multiple destroys in a single frame TODO: implement a real pooling system
    }

    private void FixedUpdate()
    {
        if (pickedUp)
        {
            if (destroyDelay > 0)
            {
                destroyDelay -= Time.deltaTime;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Pickup();
        }
    }

    /// <summary>
    /// Restores HP to the player if possible and destroys the pickup.
    /// </summary>
    public void Pickup()
    {
        if (playerStats.HP + restoreAmount > playerStats.maxHP)
        {
            playerStats.HP = playerStats.maxHP;
        }
        else
        {
            playerStats.HP += restoreAmount;
        }

        Destroy();
    }	

    /// <summary>
    /// Destroys the pickup.
    /// </summary>
    public void Destroy()
    {
        // Hide object and play pickup sound
        if (pickupSound)
        {
            audioSource.PlayOneShot(pickupSound, 1);
        }

        rend.enabled = false;
        pickedUp = true;
    }
}