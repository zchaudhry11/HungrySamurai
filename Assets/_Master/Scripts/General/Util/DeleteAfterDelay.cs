using UnityEngine;

public class DeleteAfterDelay : MonoBehaviour
{
    public float deleteTimer = 1.0f;

    private void FixedUpdate()
    {
        if (deleteTimer > 0)
        {
            deleteTimer -= Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}