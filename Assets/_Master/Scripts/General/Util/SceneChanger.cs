using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour 
{
    public string targetScene = "";
    public Vector3 targetSpawn = Vector3.zero;

    public float sceneChangeDelay = 1.00f;

    public AudioClip activateSound;

    private bool activating = false;
    private bool opened = false;

    private Animator animator;
    private AudioSource audioSource;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        audioSource = this.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (activating)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {

                if (animator)
                {
                    animator.SetBool("Opened", true);
                    opened = true;

                    if (audioSource)
                    {
                        if (activateSound)
                        {
                            audioSource.PlayOneShot(activateSound);
                        }
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (opened)
        {
            if (sceneChangeDelay > 0)
            {
                sceneChangeDelay -= Time.deltaTime;
            }
            else
            {
                // Write player data to file
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                DontDestroyOnLoad(player);

                // Change scene
                SceneManager.LoadScene(targetScene);
                player.transform.position = targetSpawn;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            activating = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            activating = false;
        }
    }
}