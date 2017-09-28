using UnityEngine;

public class Camera2D : MonoBehaviour 
{
    public Transform target;

    #region CAMERA PARAMETERS
    public float smoothTimeX = 0.05f;
    public float smoothTimeY = 0.05f;
    public Vector3 offset = Vector3.zero;
    private Vector2 velocity;
    #endregion

    #region CAMERA BOUNDS
    public bool bounds;
    public Vector3 minCamPos;
    public Vector3 maxCamPos;
    #endregion

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        float posX = Mathf.SmoothDamp(this.transform.position.x + offset.x, target.transform.position.x, ref velocity.x, smoothTimeX);
        float posY = Mathf.SmoothDamp(this.transform.position.y + offset.y, target.transform.position.y, ref velocity.y, smoothTimeY);

        this.transform.position = new Vector3(posX, posY, this.transform.position.z+offset.z);

        if (bounds)
        {
            this.transform.position = new Vector3(
                Mathf.Clamp(this.transform.position.x, minCamPos.x, maxCamPos.x),
                Mathf.Clamp(this.transform.position.y, minCamPos.y, maxCamPos.y),
                Mathf.Clamp(this.transform.position.z, minCamPos.z, maxCamPos.z)
            );
        }
    }
}