using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityStandardAssets.CrossPlatformInput;

public class ParkFakeDoor : MonoBehaviour {

    [SerializeField] AudioClip collapseSound;
    [SerializeField] AudioClip fallSound;


    GameObject playerChar;
    CapsuleCollider2D playerCollider;
    BoxCollider2D collider;
    GameObject foreground;
    Tilemap tileMap;
    List<Vector3> bridgeTiles;
    ParkLevel park;
    AudioSource bgMusic;
    
    bool playerOpenedDoorWithAllKeys = false;
    bool bridgeIsCollapsing = false;

	void Start () {
        collider = GetComponent<BoxCollider2D>();
        playerChar = GameObject.FindGameObjectWithTag("Player");
        playerCollider = playerChar.GetComponent<CapsuleCollider2D>();
        foreground = GameObject.FindGameObjectWithTag("Foreground");
        tileMap = foreground.GetComponent<Tilemap>();
        setBridgeTiles();
        park = GameObject.FindGameObjectWithTag("Level").GetComponent<ParkLevel>();
        bgMusic = GameObject.FindGameObjectWithTag("BG Music").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update () {
        if (collider.IsTouching(playerCollider) && CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            if (park.lunaKey == 4)
            {
                playerOpenedDoorWithAllKeys = true;
            }
			else {
				GetComponent<DialogTrigger>().TriggerEvent();
			}
            
        }

        if (playerOpenedDoorWithAllKeys && !bridgeIsCollapsing)
        {
            
            StartCoroutine(collapseBridge());
        }
       
    }

    private IEnumerator collapseBridge()
    {
        bridgeIsCollapsing = true;

        if (bgMusic)
        {
            bgMusic.mute = true;
        }

        var playerControl = playerChar.GetComponent<PlayerControl>();
        playerControl.animator.SetBool("isRunning", false);
        playerControl.animator.SetBool("Climbing", false);
        playerControl.animator.SetBool("onLadder", false);
        playerControl.animator.SetTrigger("isIdle");
        playerControl.canReceiveInput = false;
        playerControl.turnOffFootsteps();
        var playerGravity = playerChar.GetComponent<Rigidbody2D>().gravityScale;
        playerChar.GetComponent<Rigidbody2D>().velocity = new Vector3(0,0,0);
        playerChar.GetComponent<Rigidbody2D>().gravityScale = 0;

        yield return new WaitForSeconds(.7f);

        if (tileMap)
        {
            float collapseDelay = 0.1f;

            foreach (var tileSpot in bridgeTiles)
            {
                tileMap.SetTile(tileMap.WorldToCell(tileSpot), null);
                AudioSource.PlayClipAtPoint(collapseSound, transform.position);
                yield return new WaitForSeconds(collapseDelay);
            }
        }

        yield return new WaitForSeconds(.5f);

        if (fallSound)
        {
            AudioSource.PlayClipAtPoint(fallSound, transform.position);
            playerChar.GetComponent<Rigidbody2D>().gravityScale = 6;
        }
    }

    private void setBridgeTiles()
    {
        if (bridgeTiles == null)
        {
            bridgeTiles = new List<Vector3>();
        }

        float startX = 112.5f;
        float endX = 119f;
        float staticY = 1f;

        for (float i = startX; i <= endX; i+= 0.5f)
        {
            Vector3 tileSpot = new Vector3(i, staticY, 0);
            bridgeTiles.Add(tileSpot);
        }
    }
}
