using UnityEngine;

public class Spikes : MonoBehaviour, IObstacle
{
    #region DEPENDENCIES
    private Controller2D playerController; // TODO: replace this with player state manager
    private Rigidbody playerRB;
    private CombatManager playerCM;
    #endregion

    #region KNOCKBACK PARAMETERS
    public int damage = 25;
    public float knockbackPower = 1.0f;
    #endregion

    #region KNOCKBACK STATE
    private bool knockbacked = false; // Raised when a target has been knocked backwards
    public float knockbackTimer = 0.5f;
    private float timer;
    #endregion

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<Controller2D>();
        playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        playerCM = GameObject.FindGameObjectWithTag("Player").GetComponent<CombatManager>();
    }

    private void FixedUpdate()
    {
        if (knockbacked)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                knockbacked = false;
                playerController.knockbacked = false; // TODO: change knockback state
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Vector3 targetDir = collision.transform.position - transform.position;

            if (!knockbacked)
            {
                Knockback(targetDir.normalized);
            }

            timer = knockbackTimer;
            knockbacked = true;
            playerController.knockbacked = true;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Vector3 targetDir = collision.transform.position - transform.position;

            if (!knockbacked)
            {
                Knockback(targetDir.normalized);
            }

            timer = knockbackTimer;
            knockbacked = true;
            playerController.knockbacked = true;
        }
    }

    /// <summary>
    /// Knocks player back upon contact and deducts HP.
    /// </summary>
    /// <param name="dir">Direction of the knockback.</param>
    public void Knockback(Vector3 dir)
    {
        playerRB.AddForce(dir * 10, ForceMode.Impulse);
        playerRB.velocity = Vector3.zero;

        playerCM.Damage(damage);
    }
}