using UnityEngine;
using UnityEngine.UI;

public class FillerDecay : MonoBehaviour
{
    // Rate at which the decaying bar matches the filled bar
    public float decayRate = 0.01f;
    public Image imageFill; // Reference to filled bar

    private float fillAmount = 1; // Fill amount of the filled bar
    private Image imageDecay;
    private bool startedDecay = false;

    private void Start()
    {
        imageDecay = this.GetComponent<Image>();
    }

    private void FixedUpdate()
    {
        // Subtract decaying bar's fill amount by decayRate until it matches the filled bar.
        if (startedDecay)
        {
            if (imageDecay.fillAmount > fillAmount) // Subtract from decay bar if reference bar is reduced
            {
                imageDecay.fillAmount -= decayRate;
            }
            else if (imageDecay.fillAmount < fillAmount) // Match bars if reference bar is increased
            {
                imageDecay.fillAmount = fillAmount;
            }
            else if (imageDecay.fillAmount == fillAmount) // Stop decay if bars are equal
            {
                startedDecay = false;
            }
        }
    }

    // Activate bar decay if a fill bar has been set as reference
    public void DecayFill()
    {
        if (imageFill)
        {
            startedDecay = true;
            fillAmount = imageFill.fillAmount;
        }
        else
        {
            Debug.LogError(imageDecay.gameObject.name + " is missing a reference to its filled bar!");
        }
    }
}