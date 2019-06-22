using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class SkeletonWarriorBehavior : AlertGuardBehavior {

    public bool ShieldMode = false;
    public bool swordOut = false;

    [SerializeField] bool canUseShield = false;
    [SerializeField] bool canChase = false;
    [SerializeField] bool canJump = false;
    [SerializeField] AudioClip shieldSound;
    [SerializeField] AudioClip slashSound;
    [SerializeField] int swordDamage = 1;
    [SerializeField] float swordKnockback = 0f;
    [SerializeField] float projectileDetectionDistance = 6f;

    public float attackRate = .417f;
    private float swordRange = 1f; //1.1


    private float nextAttack = 0.0f;

    private PolygonCollider2D swordCollider;
    private BoxCollider2D shieldCollider;
    private BoxCollider2D bodyCollider;


    // Update is called once per frame
    protected override void Start()
    {
        base.Start();
        swordCollider = GetComponent<PolygonCollider2D>();
        swordCollider.enabled = false;

        var boxColliders = GetComponents<BoxCollider2D>();
        foreach (var boxCollider in boxColliders)
        {
            if (boxCollider.isTrigger)
            {
                shieldCollider = boxCollider;
            }

            else if (!boxCollider.isTrigger)
            {
                bodyCollider = boxCollider;
            }
        }

        shieldCollider.enabled = false;


    }

    protected override void Update () {
        //if (Input.GetButton("Fire3"))
        //if (CrossPlatformInputManager.GetButtonDown("Fire3"))
        //{
        //    Jump();

        //}

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



    private void MoveAlongPlatform()
    {

        if (!CloseToJumpEdge())
        {
            if (Time.time - lastMoved > 2f && isAlerted && canJump)
            {
                if (canJump)
                {
                    Jump();
                }
              
            }

            MoveForward();
        }
        else
        {
            if (canSeePlayer() && isAlerted)
            {
                //do nothing
                //rigidBody2D.velocity = new Vector2(0, 0);
                if (canJump)
                {
                    Jump();
                }
               
            }

            else
            {
                MoveBackward();
            }
        }
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
                if (!guardMode)
                {
                    if (Time.time - lastMoved > 2f)
                    {
                        //var localScaleX = transform.localScale.x;
                        //var localScaleY = transform.localScale.y;
                        //transform.localScale = new Vector2(-Math.Abs(localScaleX), localScaleY);

                        MoveBackward();
                    }

                    MoveAlongPlatformAware();

                    //MoveForward();
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
                Debug.Log("Skeleton: Calling face player while alerted");
                FacePlayer();
            }
            else
            {
                //Debug.Log("Skeleton: cannot see player while alerted");
            }

            bool canSeeProj = projectileIncoming();

            if (canSeePlayer())
            {
             
                lastSeenPlayerTime = currentTime;

                if (ShieldMode)
                {
                    return;
                }


                // Attack if player is in range

                if (isInSwordRange())
                {
                    //Debug.Log("In sword range");
                    if (currentTime > nextAttack)
                    {
                        swordCollider.enabled = true;
                        nextAttack = currentTime + attackRate;

                        if (!player.isDying && !player.isRespawning)
                        {
                            Attack();
                        }
                    }
                }
                else
                {

                    swordCollider.enabled = false;
                    isAttacking = false;

                    if (canSeeProj && canUseShield)
                    {
                        ActivateShield();
                        Invoke("ShieldAdvance", 1f);
                    }
                    else
                    {
                        MoveAlongPlatformAware();
                    }
                }
            }

            else
            {
                if (ShieldMode)
                {
                    DeactivateShield();
                }

                var lastSeenPlayerDiff = currentTime - lastSeenPlayerTime;

                MoveAlongPlatformAware();
                

                
                // TODO - MoveTowardsPlayer();
            }
        }
    }

    private void MoveAlongPlatformAware()
    {
        var enemy = CloseToEnemyFront();
        if (enemy)
        {
            if (enemy.tag.Contains("Skeleton"))
            {
                if (Math.Sign(transform.localScale.x) != Math.Sign(enemy.transform.localScale.x)) {
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

    private void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        rigidBody2D.velocity = new Vector2(0, 0);

        animator.SetBool("Walking", false);
        animator.SetBool("Attacking", true);

        AudioSource.PlayClipAtPoint(slashSound, transform.position);
        Invoke("hideSwordCollider", attackRate);



    }

    private bool isInSwordRange()
    {
        var xDistance = player.transform.position.x - transform.position.x;
        var yDistance = player.transform.position.y - transform.position.y;
        return Mathf.Abs(xDistance) <= swordRange && Math.Abs(yDistance) <= 2f;
    }

    private void SlashPlayer()
    {
        Debug.Log("SlashPlayer() called");
        var playerRigidBody = player.GetComponent<Rigidbody2D>();

        if (!player.godMode)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<HealthIndicator>().DecreaseHealth(swordDamage);
        }

        //var xDistance = transform.position.x - player.transform.position.x;

        //if (xDistance >= 0)
        //{
        //    //Knock back player left
        //    playerRigidBody.AddForce(Vector2.left * swordKnockback);
        //}
        //else
        //{
        //    playerRigidBody.AddForce(Vector2.right * swordKnockback);
        //}
    }

    private bool projectileIncoming()
    {
        var center = transform.GetComponent<Renderer>().bounds.center;


        int projectileLayerIndex = LayerMask.NameToLayer("Player Projectile");
        int layerMask = (1 << projectileLayerIndex);

        var direction = Vector2.left;
        if (isFacingRight())
        {
            direction = Vector2.right;
        }

        var hit = Physics2D.Raycast(center, direction, projectileDetectionDistance, layerMask);

        if (hit)
        {
            Debug.DrawLine(center, hit.point, Color.blue, 2f, false);
            if (hit.collider != null)
            {
                // check if projectile is going towards skeleton
                var projRigidBody = hit.collider.GetComponent<Rigidbody2D>();
                var xVelocity = projRigidBody.velocity.x;

                var facingRight = isFacingRight();

                if (Mathf.Abs(xVelocity) > 0)
                {
                    if (facingRight && Mathf.Sign(xVelocity) == 1)
                    {
                        return false;
                    }

                    else if (!facingRight && Mathf.Sign(xVelocity) == -1)
                    {
                        return false;
                    }

                    return true;
                }

               
            
            }
        }
        return false;
    }

    protected override void CheckUnalert()
    {
        var currentTime = Time.time;
        // Check if enemy should be un-alerted 

        var lastSeenDiff = Time.time - lastSeenPlayerTime;

    
        if (lastSeenDiff >= alertTimer)
        {
                Debug.Log("Enemy out of range for extended time period.");
                Unalert();
            
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        var name = collision.gameObject.name;
        var collider = collision.gameObject.GetComponent<Collider2D>();

        

        if (isAttacking)
        {
            //Slash Xin with sword
            Debug.Log("Attacking Xin with sword");

            if (name == playerName && collider.GetType() == typeof(CapsuleCollider2D))
            {
                if (!player.isHurt && !player.isDying && !player.isRespawning)
                {
                    SlashPlayer();
                    player.isHurt = true;
                    player.xinHurtEffect();
                }
              

            }
        }
        
        
        // Block
        else if (ShieldMode)
        {
            var projectile = collision.gameObject.GetComponent<Projectile>();
            if (projectile)
            {
                Destroy(collision.gameObject);

                if (shieldSound)
                {
                    AudioSource.PlayClipAtPoint(shieldSound, transform.position);
                }

            }
            else
            {
                // do nothing
            }
         
        }
        else
        {

            if (collision.gameObject.tag == "Wall")
            {
                if (!isAlerted || !canSeePlayer())
                {
                    MoveBackward();
                }
                else
                {

                }
            }

        

            if (name == playerName && collider.GetType() == typeof(CapsuleCollider2D))
            {
                Debug.Log("Collision with Xin");
                var currentXVelocity = rigidBody2D.velocity.x;
                var currentYVelocity = rigidBody2D.velocity.y;
                //Debug.Log("Xin collision velocity " + currentXVelocity);

                //Knockback and damage Xin
                
                //if (swordOut)
                //{
                //    KnockbackPlayer();
                //    DamagePlayer();
                   
                   
                //}

            }
        }   
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        //Debug.Log("Test collision - " + collision.GetType());
        

        var projectile = collision.gameObject.GetComponent<Projectile>();
        if (projectile && !isDying)
        {

            Hurt(projectile.GetDamage());
        }
        else
        {
            HandlePlayerCollision(collision);
        }
    }

    private void hideSwordCollider()
    {
        swordCollider.enabled = false;
    }

    private void ActivateShield()
    {
        ShieldMode = true;
        animator.SetBool("Shield", true);
        shieldCollider.enabled = true;
    }

    private void DeactivateShield()
    {
        ShieldMode = false;
        animator.SetBool("Shield", false);
        shieldCollider.enabled = false;
    }

    private void ToggleShield()
    {
        if (ShieldMode)
        {
            DeactivateShield();
        }

        else
        {
            ActivateShield();
        }
    }

    private void ShieldAdvance()
    {
        DeactivateShield();

      
        MoveAlongPlatformAware();
        
        
    }

  


}
