using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Bouncer : MonoBehaviour
{

    [SerializeField] public AudioClip bounceSound;
    [SerializeField] Vector3 bounceVelocity;
    [SerializeField] float bounceGravity;

    private GameObject playerChar;
    private float originalGravity;
    private float speed = 5f;
    private bool launching = false;

    private int currentIndex = 0;
    private Transform firstPosition;

    // Use this for initialization
    void Start()
    {
        playerChar = GameObject.FindGameObjectWithTag("Player");
        originalGravity = playerChar.GetComponent<Rigidbody2D>().gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (launching)
        {
            // check player landed
            var velocity = playerChar.GetComponent<Rigidbody2D>().velocity;
            if (velocity.x == 0 && velocity.y == 0)
            {
                playerChar.GetComponent<PlayerControl>().canReceiveInput = true;
                playerChar.GetComponent<Rigidbody2D>().gravityScale = originalGravity;
                launching = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            var localScaleX = collision.gameObject.transform.localScale.x;
            var localScaleY = collision.gameObject.transform.localScale.y;


            playerChar.GetComponent<Rigidbody2D>().velocity = bounceVelocity;
            playerChar.GetComponent<PlayerControl>().canReceiveInput = false;
            playerChar.GetComponent<Rigidbody2D>().gravityScale = bounceGravity;

            if (bounceVelocity.x < 0 && localScaleX > 0)
            {
                playerChar.transform.localScale = new Vector3(-1 * localScaleX, localScaleY);
            }

            launching = true;

            if (bounceSound)
            {
                AudioSource.PlayClipAtPoint(bounceSound, transform.position);
            }
        }
    }
}
