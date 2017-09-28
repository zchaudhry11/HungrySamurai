using UnityEngine;

public class Burn : MonoBehaviour 
{
    public float transitionSpeed = 1.0f;
    public bool activated = false;
    public Material[] materials;

    private float sliceAmt = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            activated = !activated;
        }

        MaterialTransition();
    }

    private void FixedUpdate()
    {
        if (activated)
        {
            if (sliceAmt < 1)
            {
                sliceAmt += (Time.deltaTime * transitionSpeed);
            }
        }
        else
        {
            if (sliceAmt > 0)
            {
                sliceAmt -= (Time.deltaTime * transitionSpeed);
            }
        }

    }

    private void MaterialTransition()
    {
        if (activated)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetFloat("_SliceAmount", sliceAmt);
            }
        }
        else
        {
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetFloat("_SliceAmount", sliceAmt);
            }
        }
    }
}