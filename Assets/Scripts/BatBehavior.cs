using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BatBehavior : EnemyBehavior
{
    [SerializeField] protected AudioClip attackSound;
    [SerializeField] protected bool idleMode = true;
    [SerializeField] public float leftFlightRange = 6f;
    [SerializeField] public float rightFlightRange = 6f;
    [SerializeField] public float downFlightRange = 5f;

    [SerializeField] protected float swoopRangeY = 2f;
    [SerializeField] protected float swoopRangeX = 1f;

    [SerializeField] protected float attackRate = 2f;

    private float nextAttack = 0.0f;

    private float leftBoundX;
    private float rightBoundX;
    private float lowerBoundY;
    private float upperBoundY;

    private bool isAttacking = false;

    private float biteRange = .1f;

    private PolygonCollider2D idleCollider;
    private BoxCollider2D walkingCollider;

    // Use this for initialization
    protected override void Start()
    {
        Initialize();

    }

    protected override void Initialize()
    {
        base.Initialize();

        var initPositionX = initialPosition.x;
        var initPositionY = initialPosition.y;

        leftBoundX = initPositionX - leftFlightRange;
        rightBoundX = initPositionX + rightFlightRange;

        lowerBoundY = initPositionY - downFlightRange;

        idleCollider = GetComponent<PolygonCollider2D>();
        walkingCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    protected override void Update()
    {
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
            var currentYVelocity = rigidBody2D.velocity.y;
            rigidBody2D.velocity = new Vector2(0, 0);
            return;
        }

        
        HandlePlayer();

    }


    private void FlyX()
    {
        var currentPositionX = transform.position.x;
        //var currentPositionY = transform.position.y;
        var playerPositionX = player.transform.position.x;
        //var playerPositionY = player.transform.position.y;

        var distanceX = Math.Abs(currentPositionX - playerPositionX);
        //var distanceY = Math.Abs(currentPositionY - playerPositionY);
        //Debug.Log("Distance x: " + distanceX + " y: " + distanceY);


        StringBuilder path = new StringBuilder();

        if (distanceX <= swoopRangeX)
        {
            //Debug.Log("Distance: " + distance);
            // dont move left or right
            path.Append("A->");
           
        }

        else if (isFacingRight() && currentPositionX >= rightBoundX)
        {
            path.Append("B->");
            if (canSeePlayer())
            {
                path.Append("C");
                //Debug.Log("Fly Path: " + path.ToString());
                StayStill();
                return;
            }
            path.Append("D->");
            MoveBackward();
        }
        else if (!isFacingRight() && currentPositionX <= leftBoundX)
        {
            path.Append("E->");
            if (canSeePlayer())
            {
                path.Append("F");
                //Debug.Log("Fly Path: " + path.ToString());
                StayStill();
                return;
            }
            MoveBackward();
        }
        else
        {
            path.Append("G");
         
            if (!closeToFlyEdgeX())
            {
                MoveForward();
            }
            else
            {
                MoveBackward();
            }

          
        }

        //Debug.Log("Fly Path: " + path.ToString());
    }

    private bool closeToFlyEdgeX()
    {
        //Get layer mask for fly Edge

        var center = transform.GetComponent<Renderer>().bounds.center;

        int invisibleLayerIndex = LayerMask.NameToLayer("Invisible");
        int layerMask = 1 << invisibleLayerIndex;
       


        //Debug.Log("Calling closeToFlyEdgeX() " + invisibleLayerIndex);
        var direction = Vector2.left;
        if (isFacingRight())
        {
            direction = Vector2.right;
        }

        var hit = Physics2D.Raycast(center, direction, 1f, layerMask);

        if (hit)
        {
            Debug.DrawLine(center, hit.point, Color.blue, 2f, false);
            if (hit.collider != null)
            {
                if (hit.collider.tag == "FlyEdge")
                {
                    Debug.Log("Close to flyedge");
                    return true;
                }



            }
        }
        return false;


    }

    private bool closeToFlyEdgeY()
    {
        //Get layer mask for fly Edge

        var center = transform.GetComponent<Renderer>().bounds.center;

        int invisibleLayerIndex = LayerMask.NameToLayer("Invisible");
        int layerMask = 1 << invisibleLayerIndex;



        //Debug.Log("Calling closeToFlyEdgeY() " + invisibleLayerIndex);
        var direction = Vector2.down;
     

        var hit = Physics2D.Raycast(center, direction, 0.5f, layerMask);

        if (hit)
        {
            Debug.DrawLine(center, hit.point, Color.blue, 2f, false);
            if (hit.collider != null)
            {
                if (hit.collider.tag == "FlyEdge")
                {
                    //Debug.Log("Close to flyedge");
                    return true;
                }



            }
        }
        return false;


    }

    private void FlyY()
    {
        var currentPositionY = transform.position.y;
        var playerPositionY = player.transform.position.y;

        var currentVelocityX = rigidBody2D.velocity.x;

        var distanceY = Math.Abs(currentPositionY - playerPositionY);


        if (closeToFlyEdgeY())
        {
            MoveUp();
            return;
        }

        if (canSeePlayer())
        {

            if (!isInSwoopRangeY())
            {
                if (currentPositionY > lowerBoundY)
                {
                    MoveDown();
                }
                else
                {
                    MoveUp();
                }
                
            }

            else
            {
                //Stay at height
               
                rigidBody2D.velocity = new Vector2(currentVelocityX, 0);
            }
        }

        else
        {
            //Move back to initial Y position
            if (currentPositionY <= initialPosition.y)
            {
                MoveUp();
            }
            else
            {
                rigidBody2D.velocity = new Vector2(currentVelocityX, 0);
            }

        }


    
    }

    private bool isInSwoopRangeY()
    {
        var currentPositionY = transform.position.y;
        var playerPositionY = player.transform.position.y;
        var distanceY = Math.Abs(currentPositionY - playerPositionY);

        return (distanceY <= swoopRangeY);
    }

    private bool isInSwoopRangeX()
    {
        var currentPositionX = transform.position.x;
        var playerPositionX = player.transform.position.x;
        var distanceX = Math.Abs(currentPositionX - playerPositionX);

        return (distanceX <= swoopRangeX);
    }

    private bool isInAttackRange()
    {
        return isInSwoopRangeX() && isInSwoopRangeY();
    }

    private void startAttack()
    {
        isAttacking = true;
    }

    private bool isInBiteRange()
    {
        var distance = Math.Abs(Vector2.Distance(player.transform.position, transform.position));
        //Debug.Log("Bite range: " + distance);
        return (distance <= biteRange);
    }


    private void StayStill()
    {
        rigidBody2D.velocity = new Vector2(0, 0);
    }

    private void Swoop()
    {
        animator.SetBool("Walking", false);
        animator.SetBool("Swooping", true);

        MoveForward();
        MoveDown();

    }

    private void MoveDown()
    {
        var xVelocity = rigidBody2D.velocity.x;
      
        rigidBody2D.velocity = new Vector2(xVelocity, -currentMoveSpeed);
     

        animator.SetBool("Walking", true);
    }

    private void MoveUp()
    {
        var xVelocity = rigidBody2D.velocity.x;

        rigidBody2D.velocity = new Vector2(xVelocity, currentMoveSpeed);


        animator.SetBool("Walking", true);
    }


    
    private void HandlePlayer()
    {
        if (idleMode)
        {
            if (!playerIsObstructed() && playerIsInRange())
            {
                FacePlayer();
                idleMode = false;
                animator.SetBool("Idle", false);
                idleCollider.enabled = false;
                walkingCollider.enabled = true;
            }

            else
            {
                return;
            }
        }

        if (canSeePlayer())
        {
            FacePlayer();

            //if (isInAttackRange() && !isAttacking)
            //{
            //    startAttack();
            //    Swoop();
            //}
        
            //else if (isAttacking)
            //{
            //    if (!isInBiteRange())
            //    {
            //        // Swoop towards player
            //        Swoop();
            //    }
            //    else
            //    {
            //        // bite
            //    }
            //}

            //else
            //{
            //    FlyX();
            //    FlyY();
            //}
        }

        if (isInBiteRange())
        {
            Debug.Log("Calling attack");
            StayStill();
        }
        else
        {

            
            FlyX();
            FlyY();
            
         

          
        }
       
    

        

     
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Bat: trigger enter");

        //var name = collision.gameObject.name;
        //var tag = collision.gameObject.tag;

        //if (name == playerName)
        //{
        //    Debug.Log("FlyEdge hit");
        //    StayStill();
        //    return;
        //}



    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
      

        var projectile = collision.gameObject.GetComponent<Projectile>();
        if (projectile && !isDying)
        {
            Destroy(collision.gameObject);
            Hurt(projectile.GetDamage());


        }
        else
        {
            HandlePlayerCollision(collision);
        }
    }

    protected override void Hurt(float damage = 0f)
    {

        var currentXVelocity = rigidBody2D.velocity.x;
        var currentYVelocity = rigidBody2D.velocity.y;


        if (canBeStunned)
        {
            float afterCollisionVelocityX = afterCollisionSpeed * Math.Sign(currentXVelocity);

            rigidBody2D.velocity = new Vector2(0, currentYVelocity);

            animator.SetBool("Hurt", true);
            isHurt = true;
        }
        else
        {

            rigidBody2D.velocity = new Vector2(0, currentYVelocity);
        }

        Health -= damage;

        colorHurt();
        if (hurtSound)
        {
            AudioSource.PlayClipAtPoint(hurtSound, transform.position);
        }


        Invoke("colorNormal", .15f);
        Invoke("unHurt", 0.75f);

    }

   

}
