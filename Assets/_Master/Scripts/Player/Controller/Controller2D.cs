using UnityEngine;

public class Controller2D : MonoBehaviour 
{
    #region MOVEMENT PARAMETERS
    [Header("Movement Parameters")]
    // Character speed 
    public float playerMaxSpeed = 5.0f;
    private float playerCurrSpeed = 0;

    public float playerCrouchSpeed = 2.0f;
    // Smoothing
    public float smoothFactor = 2;

    // Clamping
    public float maxVelocity = 100.0f;
    #endregion

    #region JUMP PARAMETERS
    [Header("Jump Parameters")]
    public float jumpForce = 400.0f; // Base force applied when character jumps
    public float gravityBoost = 3.0f; // Vertical speed boost applied as character falls

    private float verticalVelocity; // Player's vertical velocity
    #endregion

    #region ATTACK PARAMETERS
    [Header("Attack Parameters")]
    public float meleeRange = 2.0f;
    public GameObject meleeEffect;
    public AudioClip meleeSound;
    private SphereCollider attackCollider;
    #endregion

    #region DODGE PARAMETERS
    [Header("Dodge Parameters")]
    public float dodgeForce = 100.0f; // Base force applied when character dodges
    public GameObject trail;
    public AudioClip dodgeSound;
    #endregion

    #region CONTROLLER STATE
    private Vector2 input;

    // Movement state
    private bool grounded = true;

    // Action state TODO: make these private and add player state manager
    public bool crouching;
    public bool dodging;
    public bool jumping;
    public bool sprinting;
    public bool knockbacked;
    public bool attacking;
    public bool hurt;
    public bool alive = true;

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
    private Animator playerAnim;
    private Rigidbody playerRB;
    private CapsuleCollider playerCol;
    private StatManager playerStats;
    private AudioSource audioSource;
    #endregion

    private void Start()
    {
        playerAnim = this.GetComponent<Animator>();
        playerRB = this.GetComponent<Rigidbody>();
        playerCol = this.GetComponent<CapsuleCollider>();
        playerStats = this.GetComponent<StatManager>();
        audioSource = this.GetComponent<AudioSource>();

        attackCollider = this.transform.GetChild(0).GetComponent<SphereCollider>();
        attackCollider.enabled = false;
    }

    private void Update()
    {
        if (alive)
        {
            ClampVelocity();
            PlayerMovement();
        }
    }

    private void FixedUpdate()
    {
        if (alive)
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
        if (input != Vector2.zero && playerCurrSpeed < 1)
        {
            playerCurrSpeed += Time.deltaTime;
        }

        // Update grounded state
        playerAnim.SetBool("Ground", grounded);

        // Pass speed to animator
        playerAnim.SetFloat("Speed", playerCurrSpeed, 0.0f, Time.deltaTime);

        // Update dodging state
        playerAnim.SetBool("Dodge", dodging);

        // Update attacking state
        playerAnim.SetBool("Attack", attacking);

        // Update hurt state
        playerAnim.SetBool("Hurt", hurt);

        // Update alive state
        playerAnim.SetBool("Alive", alive);

        // Update crouch State
        //playerAnim.SetBool("Crouch", crouching);

        // Update jump state
        playerAnim.SetFloat("vSpeed", playerRB.velocity.y);
    }

    private void PlayerMovement()
    {
        if (!knockbacked && !attacking)
        {
            Vector3 moveDirection = Vector3.zero;

            moveDirection.x = input.x;
            moveDirection.z = input.y;

            if (input == Vector2.zero)
            {
                playerCurrSpeed = 0;
            }

            if (moveDirection.x > 0 && !playerFacingRight)
            {
                Flip();
            }
            else if (moveDirection.x < 0 && playerFacingRight)
            {
                Flip();
            }

            // Set target speed based on controller state
            float targetSpeed = playerMaxSpeed;

            if (crouching)
            {
                targetSpeed = playerCrouchSpeed;
            }

            // Translate position with raw input and clamp rigidbody velocity to maintain predictability
            transform.Translate(moveDirection * targetSpeed * Time.deltaTime);
            playerRB.velocity = Vector3.ClampMagnitude(playerRB.velocity, playerMaxSpeed);
        }
    }

    /// <summary>
    /// Reads movement input from keyboard. TODO: add controller support
    /// </summary>
    private void GetDeviceInput()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    /// <summary>
    /// Change how input is used based on ground or air movement
    /// </summary>
    private void HandlePlayerInput()
    {
        if (grounded)
        {
            if (alive)
            {
                GetDeviceInput();

                //Crouching();
                //Jumping();
                Dodging();
                Attacking();
            }
        }
        else // Player is in air
        {
            // TODO: Implement real air control and change this
            GetDeviceInput();
        }
    }

    /// <summary>
    /// Handles player crouching state
    /// </summary>
    private void Crouching()
    {
        if (!sprinting && grounded)
        {
            crouching = Input.GetKey(KeyCode.LeftControl);
        }
    }

    /// <summary>
    /// Handles player jumping state
    /// </summary>
    private void Jumping()
    {
        /*
        if (grounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumping = true;
                playerAnim.SetBool("Ground", false);

                // Add vertical force
                playerRB.AddForce(new Vector2(0, jumpForce));
            }
            else
            {
                jumping = false;
            }
        }
        */
    }

    private void Dodging()
    {
        if (grounded && !dodging)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                dodging = true;

                // Enable trail child on player
                if (trail)
                {
                    trail.SetActive(true);
                }

                if (dodgeSound)
                {
                    audioSource.PlayOneShot(dodgeSound);
                }

                dodgeTimer = dodgeLength;
                playerRB.AddForce(input * dodgeForce, ForceMode.Impulse);
                playerRB.velocity = Vector3.zero;
            }
        }
    }

    private void Attacking()
    {
        if (grounded && !dodging && !knockbacked && !attacking)
        {
            if (Input.GetMouseButtonDown(0))
            {
                attacking = true;
                attackTimer = attackLength;

                // Raycast forward to find enemy
                Vector3 origin = this.transform.position;
                Vector3 target = this.transform.position;

                if (playerFacingRight)
                {
                    target.x += meleeRange;

                    // Slash effect
                    if (meleeEffect)
                    {
                        Vector3 slashPosition = target;
                        slashPosition.x -= meleeRange/1.25f;
                        slashPosition.y -= 0.25f;

                        GameObject slash = Instantiate(meleeEffect, slashPosition, meleeEffect.transform.rotation);
                    }
                }
                else
                {
                    target.x -= meleeRange;

                    // Slash effect
                    if (meleeEffect)
                    {
                        Vector3 slashPosition = target;
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

                /*
                RaycastHit hit;
                if (Physics.Raycast(origin, target, out hit))
                {
                    // If an enemy was hit
                    if (hit.transform.gameObject.tag == "Enemy")
                    {
                        CombatManager enemyCM = hit.transform.gameObject.GetComponent<CombatManager>();

                        // If enemy is a combatable object, calculate and deal damage to it
                        if (enemyCM)
                        {
                            int damage = playerStats.GetDamagePower();
                            print(damage);

                            enemyCM.Damage(damage);
                        }
                    }
                }
                */
                attackCollider.enabled = true;
                //Debug.DrawLine(origin, target, Color.red, 3);
            }
        }
    }

    /// <summary>
    /// Sends player to hurt state.
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
    /// Check if player is currently on the ground
    /// </summary>
    private void CheckGrounded()
    {
        GetGroundDistance();

        if (groundDist <= 0.1f)
        {
            grounded = true;
            playerRB.velocity = Vector3.ProjectOnPlane(playerRB.velocity, groundHitPoint.normal); // Fix bouncing on ramps
        }
        else
        {
            if (groundDist >= groundCheckDist)
            {
                grounded = false;
                verticalVelocity = playerRB.velocity.y;

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
        if (dodgeTimer > 0)
        {
            dodgeTimer -= Time.deltaTime;
        }
        else
        {
            if (dodging)
            {
                dodging = false;

                if (trail)
                {
                    trail.SetActive(false);
                }
            }
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            if (attacking)
            {
                playerRB.velocity = Vector3.zero;
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
    /// Clamps the player's velocity in every direction based on maxVelocity.
    /// </summary>
    private void ClampVelocity()
    {
        playerRB.velocity = Vector3.ClampMagnitude(playerRB.velocity, maxVelocity);
    }

    /// <summary>
    /// Flip player's sprite
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