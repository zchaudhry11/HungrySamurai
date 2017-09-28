using UnityEngine;

public class AlertTrigger : MonoBehaviour
{
    public bool encounteredPlayer = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            encounteredPlayer = true;
        }
    }
}