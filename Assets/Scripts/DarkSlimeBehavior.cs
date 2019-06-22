using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkSlimeBehavior : EnemyBehavior {


    // Use this for initialization

    // Update is called once per frame
    

	protected override void Update () {

        if (deathParticlesShown)
        {
            var particlePosition = GetDeathParticlePosition();
            deathParticlesLocal.transform.position = particlePosition;
        }

        if (isFading)
        {
            FadeAway();
        }


        if (setToDie())
        {
            if (!isDying)
            {
                Die();
            }
            return;
        }

        if (isHurt)
        {
            //do stuff here while hurt 
            return;
        }

        MoveForward();
	}
 



    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Skeleton: OnTriggerEnter2D");

        var localScaleX = transform.localScale.x;
        var localScaleY = transform.localScale.y;

        var tag = collision.gameObject.tag;

        if (tag == "Wall" || tag == "JumpEdge")
        {
            MoveBackward();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var projectile = collision.gameObject.GetComponent<Projectile>();
        if (projectile && !isDying)
        {

            Hurt(projectile.GetDamage());
        }
        else
        {
            // Check if collided with xin
            HandlePlayerCollision(collision);


        }
    }





}
