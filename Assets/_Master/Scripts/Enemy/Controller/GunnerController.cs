using UnityEngine;

public class GunnerController : MonoBehaviour
{
    #region BURN PARAMETERS
    public AudioClip burnSound;
    public float transitionSpeed = 1.0f;
    public bool activated = false;
    private float sliceAmt = 0;
    private bool burned = false;
    #endregion

    #region BULLET PARAMETERS
    public GameObject bullet;
    public AudioClip fireSound;
    #endregion

    #region STATE TIMERS
    [Header("State Timers")]
    public float attackLength = 1.0f;
    private float attackTimer = 0;
    private bool attacking = false;
    public bool alive = true;
    #endregion

    #region DEPENDENCIES
    private Animator anim;
    private StatManager stats;
    private AudioSource audioSource;
    #endregion

    private void Start()
    {
        anim = this.GetComponent<Animator>();
        stats = this.GetComponent<StatManager>();
        audioSource = this.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!attacking && alive)
        {
            // Shoot bullet at target
            if (bullet)
            {
                GameObject bulletClone = Instantiate(bullet, this.transform.position, bullet.transform.rotation) as GameObject;
                bulletClone.GetComponent<Rigidbody>().AddForce(new Vector3(-50,0,0), ForceMode.Impulse);
                attacking = true;
                attackTimer = attackLength;
                anim.SetBool("Attack", true);

                if (fireSound)
                {
                    AudioSource.PlayClipAtPoint(fireSound, this.transform.position);
                }
            }
        }
    }

    private void Burn()
    {
        if (activated)
        {
            if (burnSound && !burned)
            {
                AudioSource.PlayClipAtPoint(burnSound, this.transform.position);
                burned = true;
            }
            
            this.GetComponent<Renderer>().material.SetFloat("_SliceAmount", sliceAmt);

            if (sliceAmt < 1)
            {
                sliceAmt += (Time.deltaTime * transitionSpeed);
            }

            if (sliceAmt >= 1)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            anim.SetBool("Attack", false);
            attacking = false;
        }

        Burn();
    }
}