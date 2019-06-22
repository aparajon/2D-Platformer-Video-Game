using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class BossSwitch : MonoBehaviour {

    [SerializeField] GameObject BossDoor;
    [SerializeField] Sprite hit;
    [SerializeField] AudioClip hitSound;
    [SerializeField] GameObject bouncer;
    [SerializeField] AudioClip fallSound;
    [SerializeField] AudioClip finalSound;


    GameObject playerChar;
    CapsuleCollider2D playerCollider;
    BoxCollider2D collider;
    SpriteRenderer spriteRenderer;
    GameObject foreground;

    AudioSource bgMusic;


    // Use this for initialization
    private Vector3 doorPosition;
    private bool switchHit = false;

    void Start () {
        collider = GetComponent<BoxCollider2D>();
        playerChar = GameObject.FindGameObjectWithTag("Player");
        playerCollider = playerChar.GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        foreground = GameObject.FindGameObjectWithTag("Foreground");
        bgMusic = GameObject.FindGameObjectWithTag("BG Music").GetComponent<AudioSource>();
        doorPosition = new Vector3(-20.3f, 65.5f, 0f);
    }
	
	// Update is called once per frame
	void Update () {
        if (collider.IsTouching(playerCollider) && CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            if (!switchHit)
            {
                StartCoroutine(StartCinematic());
            }
           
        }
    }

    private IEnumerator StartCinematic()
    {
        switchHit = true;
        AudioSource.PlayClipAtPoint(hitSound, transform.position);

        //Change sprite
        spriteRenderer.sprite = hit;

        var playerControl = playerChar.GetComponent<PlayerControl>();
        playerControl.animator.SetBool("isRunning", false);
        playerControl.animator.SetBool("Climbing", false);
        playerControl.animator.SetBool("onLadder", false);
        playerControl.animator.SetTrigger("isIdle");
        playerControl.canReceiveInput = false;
        playerControl.turnOffFootsteps();
        var playerGravity = playerChar.GetComponent<Rigidbody2D>().gravityScale;
        playerChar.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);

        yield return new WaitForSeconds(.5f);
        var skeleton = GameObject.FindGameObjectWithTag("Skeleton Cine");
        skeleton.GetComponent<SkeletonRangerBehavior>().freeze = true;
        yield return new WaitForSeconds(.1f);
        var bouncerInGame = Instantiate(bouncer, skeleton.transform.position + new Vector3(0,-0.5f,0), Quaternion.identity);
        bouncerInGame.transform.Rotate(0, 0, 270);
        if (bgMusic)
        {
            bgMusic.mute = true;
        }


        yield return new WaitForSeconds(.5f);

        skeleton.GetComponent<SkeletonRangerBehavior>().freeze2 = true;
        skeleton.GetComponent<SkeletonRangerBehavior>().animator.SetBool("Death", true);
        yield return new WaitForSeconds(.1f);
        skeleton.GetComponent<Rigidbody2D>().gravityScale = 0.75f;
        skeleton.GetComponent<Rigidbody2D>().velocity = new Vector3(-5, 15, 0);
        var bounceSound = bouncer.GetComponent<Bouncer>().bounceSound;
      
        AudioSource.PlayClipAtPoint(bounceSound, transform.position);

        yield return new WaitForSeconds(1.25f);
        skeleton.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
        skeleton.transform.position = new Vector3(-23.58f, 73.61f, 0);
        yield return new WaitForSeconds(1f);
        AudioSource.PlayClipAtPoint(fallSound, transform.position);
        yield return new WaitForSeconds(2.75f);

       
        if (finalSound)
        {
            AudioSource.PlayClipAtPoint(finalSound, transform.position);
        }
        
        Instantiate(BossDoor, doorPosition, Quaternion.identity);

        playerControl.canReceiveInput = true;
    }

}
