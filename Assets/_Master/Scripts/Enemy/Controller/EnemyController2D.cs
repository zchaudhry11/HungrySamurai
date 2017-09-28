using UnityEngine;
using UnityEngine.AI;

public class EnemyController2D : MonoBehaviour
{
    #region MOVEMENT PARAMETERS
    [Header("Movement Parameters")]
    // Character speed 
    public float maxSpeed = 5.0f;
    private float currSpeed = 0;

    // Clamping
    public float maxVelocity = 100.0f;
    #endregion

    #region JUMP PARAMETERS
    [Header("Jump Parameters")]
    public float gravityBoost = 3.0f; // Vertical speed boost applied as character falls

    private float verticalVelocity; // Player's vertical velocity
    #endregion

    #region ATTACK PARAMETERS
    [Header("Attack Parameters")]
    public float meleeRange = 2.0f;
    public GameObject meleeEffect;
    public AudioClip meleeSound;
    public bool damageOnCollision = false;
    private SphereCollider attackCollider;
    #endregion

    #region NAVIGATION PARAMETERS
    public GameObject target; //todo: set target as player when player gets close
    private AlertTrigger alertTrigger;

    private Vector3 previousPosition;
    #endregion

    #region CONTROLLER STATE
    private Vector2 input;

    // Movement state
    private bool grounded = true;

    // Action state TODO: make these private and add player state manager
    public bool crouching;
    public bool knockbacked;
    public bool attacking;
    public bool hurt;
    public bool alive = true;
    public bool targetAlive;

    // Sprite
    private bool playerFacingRight = true;
    #endregion

    #region STATE TIMERS
    [Header("State Timers")]
    public float dodgeLength = 0.40f;
    private float dodgeTimer = 0;
    public float attackLength = 0.1f;
    private float attackTimer = 0;
    public float hurtLength = 0.2f;
    private float hurtTimer = 0;
    #endregion

    #region Layers
    [Header("Layers")]
    // Ground
    private float groundDist;
    private RaycastHit groundHitPoint;
    public LayerMask groundLayer = 1 << 0;
    public float groundCheckDist = 0.5f;
    #endregion

    #region DEPENDENCIES
    private Animator enemyAnim;
    private Rigidbody enemyRB;
    private CapsuleCollider enemyCol;
    private StatManager enemyStats;
    private NavMeshAgent agent;
    private AudioSource audioSource;
    #endregion

    private void Start()
    {
        enemyAnim = this.GetComponent<Animator>();
        enemyRB = this.GetComponent<Rigidbody>();
        enemyCol = this.GetComponent<CapsuleCollider>();
        enemyStats = this.GetComponent<StatManager>();
        agent = this.GetComponent<NavMeshAgent>();
        audioSource = this.GetComponent<AudioSource>();

        // temp
        target = GameObject.FindGameObjectWithTag("Player");
        agent.updateRotation = false;

        attackCollider = this.transform.GetChild(0).GetComponent<SphereCollider>();
        attackCollider.enabled = false;

        alertTrigger = this.transform.GetChild(1).GetComponent<AlertTrigger>();
    }

    private void Update()
    {
        // If player found, chase player
        if (alertTrigger.encounteredPlayer)
        {
            targetAlive = target.GetComponent<Controller2D>().alive;

            if (alive && targetAlive)
            {
                target = GameObject.FindGameObjectWithTag("Player");


                ClampVelocity();
                PlayerMovement();

                if (target)
                {
                    agent.destination = target.transform.position;
                }
                else
                {
                    agent.Stop();
                    currSpeed = 0;
                    enemyAnim.SetBool("Attack", false);
                    enemyAnim.SetFloat("Speed", 0);
                }

            }
            else
            {
                agent.Stop();
                enemyRB.isKinematic = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (alive && targetAlive)
        {
            CheckGrounded();
            UpdateAnimator();
            UpdateStateTimers();
        }
    }

    private void LateUpdate()
    {
        if (alive)
        {
            HandlePlayerInput();
        }
    }

    private void UpdateAnimator()
    {
        // If player is moving, update current speed to transition animation from walk to run
        if (input != Vector2.zero && currSpeed < 1)
        {
            currSpeed += Time.deltaTime;
        }

        // Update grounded state
        enemyAnim.SetBool("Ground", grounded);

        // Pass speed to animator
        enemyAnim.SetFloat("Speed", currSpeed, 0.0f, Time.deltaTime);

        // Update attacking state
        enemyAnim.SetBool("Attack", attacking);

        // Update hurt state
        enemyAnim.SetBool("Hurt", hurt);

        // Update jump state
        enemyAnim.SetFloat("vSpeed", enemyRB.velocity.y);
    }

    private void PlayerMovement()
    {
        if (!knockbacked && !attacking)
        {
            Vector3 moveDirection = Vector3.zero;

            moveDirection.x = agent.velocity.x;
            moveDirection.z = agent.velocity.y;

            if (input == Vector2.zero)
            {
                //currSpeed = 0;
            }

            currSpeed = Mathf.Abs(agent.velocity.x);

            if (moveDirection.x > 0 && !playerFacingRight)
            {
                Flip();
            }
            else if (moveDirection.x < 0 && playerFacingRight)
            {
                Flip();
            }

            // Set target speed based on controller state
            float targetSpeed = maxSpeed;

            // Translate position with raw input and clamp rigidbody velocity to maintain predictability
            //transform.Translate(moveDirection * targetSpeed * Time.deltaTime);
            //enemyRB.velocity = Vector3.ClampMagnitude(enemyRB.velocity, maxSpeed);
        }
    }

    /// <summary>
    /// Reads movement input
    /// </summary>
    private void GetDeviceInput()
    {
        //input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    /// <summary>
    /// Change how input is used based on ground or air movement
    /// </summary>
    private void HandlePlayerInput()
    {
        if (grounded)
        {
            //GetDeviceInput();

            Attacking();
        }
        else // Enemy is in air
        {
            //GetDeviceInput();
        }
    }

    private void Attacking()
    {
        if (grounded && !knockbacked && !attacking && alive && targetAlive)
        {
            if (Vector3.Distance(this.transform.position, target.transform.position) < 1.0f)
            {
                agent.Stop();
                
                attacking = true;
                attackTimer = attackLength;

                // Raycast forward to find enemy
                Vector3 origin = this.transform.position;
                Vector3 slashTarget = this.transform.position;

                if (playerFacingRight)
                {
                    slashTarget.x += meleeRange;

                    // Slash effect
                    if (meleeEffect)
                    {
                        Vector3 slashPosition = slashTarget;
                        slashPosition.x -= meleeRange / 1.25f;
                        slashPosition.y -= 0.25f;

                        GameObject slash = Instantiate(meleeEffect, slashPosition, meleeEffect.transform.rotation);
                    }
                }
                else
                {
                    slashTarget.x -= meleeRange;

                    // Slash effect
                    if (meleeEffect)
                    {
                        Vector3 slashPosition = slashTarget;
                        slashPosition.x += meleeRange / 1.25f;
                        slashPosition.y -= 0.25f;

                        GameObject slash = Instantiate(meleeEffect, slashPosition, Quaternion.Euler(0, -180, 0));
                    }
                }

                // Play slash sound
                if (audioSource)
                {
                    if (meleeSound)
                    {
                        audioSource.PlayOneShot(meleeSound, 1);
                    }
                }

                slashTarget = target.transform.position;

                attackCollider.enabled = true;
                //Debug.DrawLine(origin, slashTarget, Color.red, 3);
            }
            else
            {
                agent.destination = target.transform.position;
                agent.Resume();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (alive && damageOnCollision)
        {
            if (collision.gameObject.tag == "Player")
            {
                CombatManager enemyCM = collision.gameObject.transform.GetComponent<CombatManager>();

                // If enemy is a combatable object, calculate and deal damage to it
                if (enemyCM)
                {
                    int damage = enemyStats.GetDamagePower();

                    enemyCM.Damage(damage);

                    Rigidbody playerRB = collision.gameObject.GetComponent<Rigidbody>();

                    Vector3 targetDir = collision.transform.position - transform.position;

                    playerRB.AddForce(targetDir.normalized * 10, ForceMode.Impulse);
                    playerRB.velocity = Vector3.zero;

                }
            }
        }
    }

    /// <summary>
    /// Sends enemy to hurt state.
    /// </summary>
    public void Hurt()
    {
        hurt = true;
        hurtTimer = hurtLength;
    }

    /// <summary>
    /// Get vertical distance to the ground
    /// </summary>
    private void GetGroundDistance()
    {
        Ray ray = new Ray(transform.position + new Vector3(0, 1.3f * 0.5f, 0), Vector3.down);
        float distance = Mathf.Infinity;

        if (Physics.Raycast(ray, out groundHitPoint, Mathf.Infinity, groundLayer)) //TODO: maybe use capsule/sphere casting
        {
            distance = transform.position.y - groundHitPoint.point.y;
            //Debug.DrawLine(ray.origin, groundHitPoint.point, Color.red, 1.5f); // Draw line to ground
        }

        groundDist = distance;
    }

    /// <summary>
    /// Check if enemy is currently on the ground
    /// </summary>
    private void CheckGrounded()
    {
        GetGroundDistance();

        if (groundDist <= 0.1f)
        {
            grounded = true;
            enemyRB.velocity = Vector3.ProjectOnPlane(enemyRB.velocity, groundHitPoint.normal); // Fix bouncing on ramps
        }
        else
        {
            if (groundDist >= groundCheckDist)
            {
                grounded = false;
                verticalVelocity = enemyRB.velocity.y;

                // Apply additional gravity over time when falling
                transform.position -= new Vector3(0, gravityBoost * Time.deltaTime, 0);
            }
            else if (groundDist < groundCheckDist)
            {
                grounded = true;
            }
        }
    }

    private void UpdateStateTimers()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            if (attacking)
            {
                enemyRB.velocity = Vector3.zero;
                input = Vector2.zero;
                attacking = false;
                attackCollider.enabled = false;
            }
        }

        if (hurtTimer > 0)
        {
            hurtTimer -= Time.deltaTime;
        }
        else
        {
            hurt = false;
        }

    }

    /// <summary>
    /// Clamps the enemy's velocity in every direction based on maxVelocity.
    /// </summary>
    private void ClampVelocity()
    {
        enemyRB.velocity = Vector3.ClampMagnitude(enemyRB.velocity, maxVelocity);
    }

    /// <summary>
    /// Flip enemy's sprite
    /// </summary>
    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        playerFacingRight = !playerFacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}