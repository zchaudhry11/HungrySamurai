using UnityEngine;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    private EnemyController2D controller;

    public float timer = 3.0f;

    private void Start()
    {
        controller = this.GetComponent<EnemyController2D>();
    }

    private void FixedUpdate()
    {
        if (controller.alive == false)
        {
            if (timer <= 0)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                timer -= Time.deltaTime;
            }
            
        }
    }
}