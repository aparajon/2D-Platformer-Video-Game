using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class SkeletonMageBehavior : AlertGuardBehavior {

    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeedX;
    [SerializeField] float projectileRate = 1f;
    [SerializeField] AudioClip projectileSound;
    [SerializeField] AudioClip summonSound;
    [SerializeField] AudioClip blockSound;
    [SerializeField] GameObject skeletonWarrior;
    [SerializeField] GameObject skeletonRanger;
    [SerializeField] float summonRate = 8f;

    private float nextFire = 0.0f;
    private float nextSummon = 0.0f;
    private bool isSummoning = false;
    public float lastSummonTime = 0.0f;

    public bool summonRangers = false;

    public List<GameObject> minionList;
   

    protected override void Start()
    {
        base.Start();
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        animator.SetBool("Standing", false);
    }

    


    // Update is called once per frame
    protected override void Update()
    {

        if (CrossPlatformInputManager.GetButtonDown("Fire3"))
        {
            //StartSummon();
        }

        foreach (var minion in minionList)
        {
            if (minion == null)
            {
                minionList.Remove(minion);
            }
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

                    MoveForward();
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
                //Debug.Log("Skeleton mage: Calling face player while alerted");
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
                if (currentTime > nextSummon && minionList.Count == 0)
                {
                    nextSummon = currentTime + summonRate;
                    StartSummon();
                }


                else if (currentTime > nextFire)
                {
                    nextFire = currentTime + projectileRate;
                    Attack();
                  
                }
            }

            else
            {
                if (isSummoning)
                {
                    return;
                } 

                var lastSeenPlayerDiff = currentTime - lastSeenPlayerTime;
                //Debug.Log("Last seen player: " + lastSeenPlayerDiff);
                if (lastSeenPlayerDiff > attackPause)
                {
                    MoveAlongPlatform();
                }
            }
        }
    }

    protected override void MoveForward()
    {
        base.MoveForward();
        animator.SetBool("Standing", false);  
    }

    protected override void MoveBackward()
    {
        base.MoveBackward();
        animator.SetBool("Standing", false);
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

    void Attack()
    {
        rigidBody2D.velocity = new Vector2(0, 0);

        animator.SetBool("Walking", false);
        animator.SetBool("Standing", false);
        animator.SetBool("Attacking", true);
        isAttacking = true;

        //spawn projectile
        //CreateFireball();
        lastAttackTime = Time.time;

        Invoke("StopAttackAnimation", 0.5f);

        if (!CloseToJumpEdge())
        {
            // Debug.Log("Attack move");
            //Invoke("MoveForward", 0.5f);
        }

        //Move back

        //Invoke("MoveBackward", 0.5f);
    }

    void CreateFireball()
    {
        //Debug.Log("Skeleton Mage: CreateFireBall called.");
        //start position front of character
        float xOffset = .75f * Mathf.Sign(transform.localScale.x);
        //Debug.Log("xOffset: " + xOffset);

        // Debug.Log("xVelocity: " + xVelocity + " yVelocity: " + yVelocity);

        Vector3 fireballOffSet = transform.position + new Vector3(xOffset, 1.25f, 0);


        //throw in direction facing
        float xVelocity = projectileSpeedX * Mathf.Sign(transform.localScale.x);
        float yVelocity = 0;

     

  
        var fireball = Instantiate(projectile, fireballOffSet, Quaternion.identity);
        

        //fireball.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity, yVelocity);
        if (transform.localScale.x > 0)
        {
            fireball.GetComponent<Rigidbody2D>().AddForce(transform.right * 300);
            fireball.transform.Rotate(0, 0, 90);
        }
        else
        {
            fireball.GetComponent<Rigidbody2D>().AddForce(-transform.right * 300);
            fireball.transform.Rotate(0, 0, -90);
        }
        AudioSource.PlayClipAtPoint(projectileSound, transform.position);
 

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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        var projectile = collision.gameObject.GetComponent<Projectile>();
        if (projectile && !isDying)
        {
            if (isSummoning)
            {

                var currentYVelocity = rigidBody2D.velocity.y;



                rigidBody2D.velocity = new Vector2(0, 0);
                

                //do block effect
                if (blockSound)
                {
                    AudioSource.PlayClipAtPoint(blockSound, transform.position);
                }

                return;
            }

            else
            {
                Hurt(projectile.GetDamage());
            }


           
        }
        else
        {
            HandlePlayerCollision(collision);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //rigidBody2D.isKinematic = false;
    }

    private void StopAttackAnimation()
    {
        if (!canSeePlayer())
        {
            animator.SetBool("Walking", true);
            animator.SetBool("Standing", false);
        }
        else
        {
            animator.SetBool("Walking", false);
            animator.SetBool("Standing", true);
        }

        
        animator.SetBool("Attacking", false);
    }

    private void StopSummoning()
    {
        if (!canSeePlayer())
        {
            animator.SetBool("Walking", true);
            animator.SetBool("Standing", false);
     
        }
        else
        {
            animator.SetBool("Walking", false);
            animator.SetBool("Standing", true);
        }


        animator.SetBool("Summon", false);
        isSummoning = false;
    }

    private void StartSummon()
    {
        if (!isSummoning)
        {
            rigidBody2D.velocity = new Vector2(0, 0);

            animator.SetBool("Walking", false);
            animator.SetBool("Standing", false);
            animator.SetBool("Summon", true);
            isSummoning = true;

            lastSummonTime = Time.time;

            Invoke("StopSummoning", 1.5f);
        }

    }

    private void Summon()
    {
        if (summonSound)
        {
            AudioSource.PlayClipAtPoint(summonSound, transform.position);
        }

        SpawnWarriors();
        //SpawnRangers();
    }

    private void SpawnWarriors()
    {
        if (skeletonWarrior)
        {
            var warriorController = skeletonWarrior.GetComponent<SkeletonWarriorBehavior>();
            //alertController.isAlerted = true;

            if (isFacingRight())
            {
                warriorController.startsRight = true;
            }
            else
            {
                warriorController.startsRight = false;
            }

            warriorController.useAlertEffect = false;
            

            float xOffset = 2 * Mathf.Sign(transform.localScale.x);

            // If too close to edge, spawn behind
            if (CloseToEdge())
            {
                xOffset *= -1;
            }

            var spawnPosition1 = transform.position + new Vector3(xOffset, 0, 0);
            var spawnPosition2 = transform.position + new Vector3(xOffset / 2, 0, 0);



            var s1 = Instantiate(skeletonWarrior, spawnPosition1, Quaternion.identity);
            var s2 = Instantiate(skeletonWarrior, spawnPosition2, Quaternion.identity);
            minionList.Add(s1);
            minionList.Add(s2);

        }
    }

    private void SpawnRangers()
    {
        if (skeletonRanger)
        {
            var rangerController = skeletonRanger.GetComponent<SkeletonRangerBehavior>();
            //alertController.isAlerted = true;

            if (isFacingRight())
            {
                rangerController.startsRight = true;
            }
            else
            {
                rangerController.startsRight = false;
            }

            rangerController.useAlertEffect = false;


            float xOffset = .5f * Mathf.Sign(transform.localScale.x);

            // If too close to edge, spawn behind
            if (CloseToEdge())
            {
                xOffset *= -1;
            }

            var spawnPosition1 = transform.position + new Vector3(xOffset, 0, 0);
            //var spawnPosition2 = transform.position + new Vector3(xOffset / 2, 0, 0);



            var s1 = Instantiate(skeletonRanger, spawnPosition1, Quaternion.identity);
            //var s2 = Instantiate(skeletonRanger, spawnPosition2, Quaternion.identity);
        }
    }
}
