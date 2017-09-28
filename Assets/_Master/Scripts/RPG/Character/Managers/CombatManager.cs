using System.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour, ICombat
{
    #region DEPENDENCIES
    private Controller2D controller;
    private EnemyController2D enemyController;
    private GunnerController gunnerController;
    private StatManager stats;
    private SpriteRenderer charRenderer;
    private Animator animator;
    #endregion

    #region ENEMY DROPS
    public GameObject coin;
    public GameObject hhPickup;
    private bool alive = true;
    #endregion

    [Header("Flash Parameters")]
    #region FLASH PARAMETERS
    public Color defaultColor;
    public Color damageColor;
    public int numFlashes = 5;
    public float flashRate = 0.1f;
    #endregion

    private void Start()
    {
        controller = this.GetComponent<Controller2D>();
        enemyController = this.GetComponent<EnemyController2D>();
        gunnerController = this.GetComponent<GunnerController>();

        stats = this.GetComponent<StatManager>();
        charRenderer = this.GetComponent<SpriteRenderer>();
        animator = this.GetComponent<Animator>();
    }

    /// <summary>
    /// Inflicts damage to the character.
    /// </summary>
    /// <param name="dmg">Points of damage to inflict.</param>
    public void Damage(int dmg)
    {
        //Defense calculations go here
        dmg -= stats.Defense;

        if (dmg > 0)
        {
            // Player hit state
            if (controller)
            {
                controller.Hurt();
            }

            // add enemy hit state

            stats.HP -= dmg;

            if (stats.HP <= 0)
            {
                animator.SetBool("Alive", false);

                if (controller)
                {
                    controller.alive = false;
                }

                if (enemyController)
                {
                    enemyController.alive = false;
                    enemyController.transform.GetChild(0).gameObject.SetActive(false);
                    enemyController.transform.GetChild(1).gameObject.SetActive(false);

                    // Spawn drops
                    if (coin && alive)
                    {
                        int numcoins = Random.Range(1, 3);

                        for (int i = 0; i < numcoins; i++)
                        {
                            Instantiate(coin, this.transform.position, this.transform.rotation);
                        }
                    }

                    if (hhPickup && alive)
                    {
                        int spawnHH = Random.Range(0, 100);

                        if (spawnHH % 5 == 0)
                        {
                            Instantiate(hhPickup, this.transform.position, this.transform.rotation);
                        }
                    }

                    alive = false;
                    this.enabled = false;
                }

                if (gunnerController)
                {
                    gunnerController.activated = true;

                    // Spawn drops
                    if (coin && alive)
                    {
                        int numcoins = Random.Range(1, 3);

                        for (int i = 0; i < numcoins; i++)
                        {
                            Instantiate(coin, this.transform.position, this.transform.rotation);
                        }
                    }

                    if (hhPickup && alive)
                    {
                        int spawnHH = Random.Range(0, 100);

                        if (spawnHH % 5 == 0)
                        {
                            Instantiate(hhPickup, this.transform.position, this.transform.rotation);
                        }
                    }

                    alive = false;
                    this.enabled = false;
                }
            }
            else
            {
                StartCoroutine(Flasher());
            }
        }
        else
        {
            print("0 dmg");
        }
    }

    /// <summary>
    /// Restores HP to the character.
    /// </summary>
    /// <param name="hp">Points of HP to restore.</param>
    public void Heal(int hp)
    {
        //Stat calculations go here

        if (hp > 0)
        {
            stats.HP += hp;
        }

    }

    /// <summary>
    /// Makes a character flash between two colors;
    /// </summary>
    private IEnumerator Flasher()
    {
        for (int i = 0; i < numFlashes; i++)
        {
            charRenderer.material.color = damageColor;
            yield return new WaitForSeconds(flashRate);
            charRenderer.material.color = defaultColor;
            yield return new WaitForSeconds(flashRate);
        }
    }
}