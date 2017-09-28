using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    private StatManager stats;
    private Controller2D controller;
    private EnemyController2D enemyController;

    private void Start()
    {
        stats = this.transform.parent.gameObject.GetComponent<StatManager>();
        controller = this.transform.parent.gameObject.GetComponent<Controller2D>();
        enemyController = this.transform.parent.gameObject.GetComponent<EnemyController2D>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player's weapon
        if (controller)
        {
            if (other.gameObject.tag == "Enemy")
            {
                CombatManager enemyCM = other.transform.gameObject.GetComponent<CombatManager>();

                // If enemy is a combatable object, calculate and deal damage to it
                if (enemyCM)
                {
                    int damage = stats.GetDamagePower();
                    //print("player dealt: " + damage + " damage.");

                    enemyCM.Damage(damage);
                }
            }
        }

        // Enemy's weapon
        if (enemyController)
        {
            if (other.gameObject.tag == "Player")
            {
                CombatManager playerCM = other.transform.gameObject.GetComponent<CombatManager>();

                // If enemy is a combatable object, calculate and deal damage to it
                if (playerCM)
                {
                    int damage = stats.GetDamagePower();
                    //print("enemy dealt: " + damage + " damage.");

                    playerCM.Damage(damage);
                }
            }
        }
    }
}