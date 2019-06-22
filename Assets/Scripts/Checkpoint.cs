using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    [SerializeField] Sprite FlagNotReached;
    [SerializeField] Sprite FlagReached;
    [SerializeField] AudioClip CheckpointSound;

    private HealthIndicator healthIndicator;
    private PlayerControl playerController;
    private BoxCollider2D collider;

    private BoxCollider2D playerFeetCollider;
    private CapsuleCollider2D playerCapsuleCollider;

    public bool checkPointReached = false;
    private GameObject flag;

	// Use this for initialization
	void Start () {
        flag = GameObject.FindGameObjectWithTag("Flag");

        healthIndicator = FindObjectOfType<HealthIndicator>();
        collider = GetComponent<BoxCollider2D>();
        var player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerControl>();
        playerFeetCollider = player.GetComponent<BoxCollider2D>();
        playerCapsuleCollider = player.GetComponent<CapsuleCollider2D>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var tag = collision.gameObject.tag;

        Debug.Log("Trigger enter checkpoint - " + tag);
        if (tag == "Player" && !checkPointReached)
        {
            checkPointReached = true;
            flag.GetComponent<SpriteRenderer>().sprite = FlagReached;
            AudioSource.PlayClipAtPoint(CheckpointSound, transform.position);
            playerController.spawnPosition = transform.position;
        }


    }



}
