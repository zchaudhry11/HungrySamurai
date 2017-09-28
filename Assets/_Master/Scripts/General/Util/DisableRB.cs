using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRB : MonoBehaviour
{
    public LayerMask groundLayer;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == groundLayer)
        {
            this.GetComponent<Rigidbody>().isKinematic = true;
            this.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
