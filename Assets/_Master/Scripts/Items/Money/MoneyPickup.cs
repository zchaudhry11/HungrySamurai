using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MoneyPickup : MonoBehaviour, IPickup
{
    #region DEPENDENCIES
    private StatManager playerStats;
    private AudioSource audioSource;
    private Renderer rend;
    #endregion

    #region STATE
    private bool pickedUp = false;
    #endregion

    #region MONEY PARAMETERS
    public int pickupValue = 1;
    #endregion

    #region PICKUP EFFECTS
    public AudioClip pickupSound;
    public float destroyDelay = 1.00f; // Seconds to wait before destroying a de-activated object

    // particle system
    #endregion

    private void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<StatManager>();
        audioSource = this.GetComponent<AudioSource>();
        rend = this.GetComponent<Renderer>();
        destroyDelay += Random.Range(0.0f, 1.5f); // Add a random offset to reduce chance of multiple destroys in a single frame TODO: implement a real pooling system
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
            if (!pickedUp)
            {
                Pickup();
            }
        }
    }

    /// <summary>
    /// Adds money to the player's wallet.
    /// </summary>
    public void Pickup()
    {
        playerStats.money += pickupValue;

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