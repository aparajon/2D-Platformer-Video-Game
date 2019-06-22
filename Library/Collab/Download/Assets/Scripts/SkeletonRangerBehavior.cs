using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonRangerBehavior : AlertGuardBehavior
{

    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeedX;
    [SerializeField] float projectileRate = 1f;
    [SerializeField] AudioClip projectileSound;

    private float nextFire = 0.0f;


    private CapsuleCollider2D bodyCollider;

    protected override void Start()
    {
        base.Start();
        bodyCollider = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    protected override void Update()
    {

        if (freeze2)
        {
            return;
        }

        if (freeze)
        {
            //Freeze for cinematic effect
            isAlerted = false;
            animator.SetBool("Walking", false);
            animator.SetBool("Attacking", false);
            animator.SetBool("Hurt", false);
            animator.SetBool("Guard", true);
            //animator.SetBool("Death", true);
            return;
        }

        var currentPosition = transform.position;
        if (currentPosition != lastPosition)
        {
            lastPosition = transform.position;
            lastMoved = Time.time;
        }

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

        adjustPositionOverlap();
        SetAlertImage();
        HandlePlayer();

    }

    private void HandlePlayer()
    {
        if (!isAlerted)
        {
            // If not alrted, alert if can see player -> is facing player, and can see player

            if (isFacingPlayer() && canSeePlayer())
            {
                Alert(useAlertEffect);
            }

            else
            {
                //MoveAlongPlatform();

                if (!guardMode)
                {

                    MoveAlongPlatformAware();
                }

            }
        }

        else
        {
            // Skeleton is alerted

            var currentTime = Time.time;

            // Check if alert image should be displayed

            CheckUnalert();

            //Face player if not behind structures
            if (!playerIsObstructed() && playerIsInRange())
            {
                //Debug.Log("Skeleton: Calling face player while alerted");
                FacePlayer();
            }
            else
            {
                //Debug.Log("Skeleton: cannot see player while alerted");
            }


            // Attack if player is in range
            if (canSeePlayer())
            {
                lastSeenPlayerTime = currentTime;

                var playerDistance = Vector2.Distance(player.transform.position, transform.position);
                Debug.Log(playerDistance);
                if (currentTime > nextFire && playerDistance >= 3f)
                {
                    nextFire = currentTime + projectileRate;
                    Attack();
                }
                else
                {
                    if (playerDistance <= 3f && !CloseToJumpEdge())
                    
                        MoveAlongPlatformAware();
                }
            }

            else
            {
                var lastSeenPlayerDiff = currentTime - lastSeenPlayerTime;
                //Debug.Log("Last seen player: " + lastSeenPlayerDiff);
                if (lastSeenPlayerDiff > attackPause)
                {
                    MoveAlongPlatformAware();
                }
            }
        }
    }

    private void MoveAlongPlatform()
    {
        if (!CloseToJumpEdge())
        {
            MoveForward();
        }
        else
        {
            MoveBackward();
        }
    }

    private void MoveAlongPlatformAware()
    {
        var enemy = CloseToEnemyFront();
        if (enemy)
        {
            if (enemy.tag.Contains("Skeleton"))
            {
                if (Math.Sign(transform.localScale.x) != Math.Sign(enemy.transform.localScale.x))
                {
                    //Facing opposite directions
                    MoveAlongPlatform();
                }

                else
                {
                    // dont advance to prevent overlap
                }

            }
            else
            {
                MoveAlongPlatform();
            }
        }
        else
        {
            MoveAlongPlatform();
        }
    }

    void Attack()
    {
        rigidBody2D.velocity = new Vector2(0, 0);

        if (boneVelocityTooHigh() || onWrongThrowSide())
        {
            return;
        }

        animator.SetBool("Walking", false);
        animator.SetBool("Attacking", true);
        isAttacking = true;

        //spawn projectile
        //ThrowBone();
        lastAttackTime = Time.time;

        //Invoke("StopAttackAnimation", 0.5f);

        if (!CloseToJumpEdge())
        {
            // Debug.Log("Attack move");
            Invoke("MoveForward", 0.5f);
        }

        //Move back

        //Invoke("MoveBackward", 0.5f);
    }

    void ThrowBone()
    {
        //Debug.Log("Skeleton: Throw bone called.");
        //start position front of character
        //float xOffset = .75f * Mathf.Sign(transform.localScale.x);
        float xOffset = 0 * Mathf.Sign(transform.localScale.x);

        //Debug.Log("xOffset: " + xOffset);

        // Debug.Log("xVelocity: " + xVelocity + " yVelocity: " + yVelocity);

        Vector3 boneOffSet = transform.position + new Vector3(xOffset, 1.25f, 0);


        //throw in direction facing
        float xVelocity = projectileSpeedX * Mathf.Sign(transform.localScale.x);
        float yVelocity = CalculateVelocityY(boneOffSet, player.transform.position, xVelocity);

       

     

        

        if (boneVelocityTooHigh() || onWrongThrowSide())
        {
            // don't throw bone
            Debug.Log("Skeleton: Throw bone canceled - " + yVelocity);
        }
        else
        {
            var bone = Instantiate(projectile, boneOffSet, Quaternion.identity);
            bone.transform.parent = transform;

            bone.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity, yVelocity);
            AudioSource.PlayClipAtPoint(projectileSound, transform.position);
        }

        Invoke("StopAttackAnimation", .2f);
    }

    private bool boneVelocityTooHigh()
    {
        float xOffset = .75f * Mathf.Sign(transform.localScale.x);
        Vector3 boneOffSet = transform.position + new Vector3(xOffset, 1.25f, 0);
        float xVelocity = projectileSpeedX * Mathf.Sign(transform.localScale.x);
        float yVelocity = CalculateVelocityY(boneOffSet, player.transform.position, xVelocity);
        if (Math.Abs(yVelocity) > 15)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool onWrongThrowSide()
    {
        var dist = transform.position.x - player.transform.position.x;

        bool onWrongSide = (transform.localScale.x < 0 && dist < 0) ||
            (transform.localScale.x > 0 && dist > 0);

        return onWrongSide;
    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Skeleton: OnCollisionEnter");

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Skeleton: OnTriggerEnter2D");

        var localScaleX = transform.localScale.x;
        var localScaleY = transform.localScale.y;

        if (collision.gameObject.tag == "Wall")
        {
            var xVelocity = rigidBody2D.velocity.x;
            // Debug.Log("Wall found. X velocity: " + xVelocity + " localScaleX: " + localScaleX);


            //transform.localScale = new Vector2(-Mathf.Sign(localScaleX) * Math.Abs(localScaleX), localScaleY);


            if (!isAlerted)
            {

                MoveBackward();
            }
            else
            {
                if (!canSeePlayer())
                {
                    MoveBackward();
                }

            }

        }

        else if (collision.gameObject.tag == "JumpEdge")
        {
            //Debug.Log("Skeleton: JumpEdge found.");


            if (!isAlerted)
            {
                // bumps into edge while roaming
                MoveBackward();
            }
            else
            {
                //if (!canAttackPlayer())
                //{
                //    Invoke("MoveBackward", 1f);
                //}
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "JumpEdge")
        {
            //Debug.Log("Skeleton: OnTriggerExit2D called for JumpEdge");
        }
    }

    private float CalculateVelocityY(Vector3 source, Vector3 target, float xVelocity)
    {
        //Calculate time via xVelocity

        float xDistance = target.x - source.x;
        float time = xDistance / xVelocity;

        //Calculate yVelocity
        float g = Physics.gravity.y;
        float yDistance = target.y - source.y;


        float yVelocity = (yDistance - (0.5f * g * time * time)) / time;

        // Debug.Log("xDistance: " + xDistance + " yDistance: " + yDistance + " time: " + time);

        return yVelocity;
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

    private void OnCollisionExit2D(Collision2D collision)
    {
        //rigidBody2D.isKinematic = false;
    }

    private void adjustPositionOverlap()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = LayerMask.GetMask("Enemies");
        filter.useLayerMask = true;
        Collider2D[] results = new Collider2D[10];
        bodyCollider.OverlapCollider(filter, results);
        foreach (var result in results)
        {
            if (result)
            {
                if (result.tag.Contains("Skeleton"))
                {
                    if (Math.Sign(transform.localScale.x) == Math.Sign(result.gameObject.transform.localScale.x))
                    {
                        float rand = UnityEngine.Random.value;
                        if (rand <= 0.5)
                        {
                            MoveBackward();

                            if (isAlerted && canSeePlayer())
                            {
                                FacePlayer();
                            }
                        }
                    }
                }
            }
        }
    }
}
