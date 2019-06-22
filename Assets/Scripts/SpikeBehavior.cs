using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBehavior : MonoBehaviour {

    private HealthIndicator healthIndicator;
    private PlayerControl playerController;
    private BoxCollider2D collider;


    private BoxCollider2D playerFeetCollider;
    private CapsuleCollider2D playerCapsuleCollider;

    private void Start()
    {
        healthIndicator = FindObjectOfType<HealthIndicator>();
        collider = GetComponent<BoxCollider2D>();
        var player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerControl>();
        playerFeetCollider = player.GetComponent<BoxCollider2D>();
        playerCapsuleCollider = player.GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        if (collider.IsTouching(playerFeetCollider) || collider.IsTouching(playerCapsuleCollider))
        {
            if (!playerController.isHurt && !playerController.isDying && !playerController.isRespawning)
            {
                playerController.isHurt = true;
                if (!playerController.godMode)
                {
                    healthIndicator.DecreaseHealth(2);
                }
                playerController.xinHurtEffect();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Skeleton")
        {
            var behavior = collision.gameObject.GetComponent<EnemyBehavior>();
            behavior.SetHealth(0);
        }
    }
}
