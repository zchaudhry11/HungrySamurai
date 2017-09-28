using UnityEngine;

public class ShopController : MonoBehaviour
{
    public AudioClip purchaseSound;

    public bool activating = false;

    private void Update()
    {
        if (activating)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                StatManager stats = player.GetComponent<StatManager>();

                if (stats.money >= 10)
                {
                    AudioSource audioSource = player.GetComponent<AudioSource>();
                    if (audioSource)
                    {
                        audioSource.PlayOneShot(purchaseSound);
                    }

                    stats.money -= 10;
                    stats.maxHP += 10;
                    stats.HP = stats.maxHP;
                    stats.Strength += 1;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            activating = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            activating = false;
        }
    }
}