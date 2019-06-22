using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehavior : MonoBehaviour {

 
    [SerializeField] protected float Health = 600f;
    [SerializeField] protected float baseMoveSpeed = 1f;
    [SerializeField] protected float edgeDetectionDistance;
    [SerializeField] protected float playerDetectionDistance = 8f;
    [SerializeField] protected bool drawDebugSightLines = true;
    [SerializeField] protected float knockbackAmount = 10000f;
    [SerializeField] protected int knockBackDamage = 1;
    [SerializeField] protected float afterCollisionSpeed = 1f;
    [SerializeField] protected float fadePerSecond = 2.5f;
    [SerializeField] public bool startsRight = true;
    [SerializeField] protected float deathParticleYOffset = .6f;

    [SerializeField] public bool freeze = false;
    [SerializeField] public bool freeze2 = false;
    [SerializeField] protected bool canBeStunned = true; 
   
    [SerializeField] protected AudioClip hurtSound;
    [SerializeField] protected AudioClip deathSound;
    [SerializeField] protected AudioClip knockbackSound;
    [SerializeField] protected GameObject deathParticles;

    public float lastHurtTime = 0.0f;
    public float lastSeenPlayerTime = 0.0f;
    public float lastMoved = 0.0f;

    protected Vector3 lastPosition;


    public float currentMoveSpeed;
    protected bool isHurt = false;
    protected bool isDying = false;
    protected bool isFading = false;
    protected bool deathParticlesShown = false;
    protected GameObject deathParticlesLocal;

    protected Rigidbody2D rigidBody2D;
    public Animator animator;
    protected PlayerControl player;
    protected SpriteRenderer spriteRenderer;

    protected string playerName = "Xin";

    protected Color defaultColor;
    protected Color hurtColor;
    protected Vector3 initialPosition;

    protected float defaultWalkingAnimSpeed;

    protected bool isColliding = false;


    // Use this for initialization
    protected virtual void Start() {
        Initialize();
    }

    // Update is called once per frame
    protected virtual void Update () {
		
	}

    protected virtual void Initialize()
    {
        currentMoveSpeed = baseMoveSpeed;

        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerControl>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        var localScaleX = transform.localScale.x;
        var localScaleY = transform.localScale.y;

        if (startsRight)
        {
            transform.localScale = new Vector2(Math.Abs(localScaleX), localScaleY);
        }
        else
        {
            transform.localScale = new Vector2(-Math.Abs(localScaleX), localScaleY);
        }

        defaultColor = spriteRenderer.color;
        hurtColor = new Color32(233, 145, 145, 255);

        initialPosition = transform.position;
        defaultWalkingAnimSpeed = animator.GetFloat("WalkSpeed");
    }

    protected bool isFacingRight()
    {
        return transform.localScale.x > 0;
    }

    protected bool isFacingPlayer()
    {
        var xDistance = transform.position.x - player.transform.position.x;
        return (Mathf.Sign(transform.localScale.x) == -Mathf.Sign(xDistance));
    }

    protected void FacePlayer()
    {
        var xDistance = transform.position.x - player.transform.position.x;

        var localScaleX = transform.localScale.x;
        var localScaleY = transform.localScale.y;

        transform.localScale = new Vector2(-1 * Mathf.Sign(xDistance) * Math.Abs(localScaleX), localScaleY);
    }


    protected virtual void MoveForward()
    {
        // Debug.Log("Move forward called");
        if (!rigidBody2D)
        {
            return;
        }

        var yVelocity = rigidBody2D.velocity.y;


        if (isFacingRight())
        {
            rigidBody2D.velocity = new Vector2(currentMoveSpeed, yVelocity);
        }
        else
        {
            rigidBody2D.velocity = new Vector2(-currentMoveSpeed, yVelocity);
        }

        animator.SetBool("Walking", true);
    }

    protected virtual void MoveBackward()
    {
        var localScaleX = transform.localScale.x;
        var localScaleY = transform.localScale.y;

        transform.localScale = new Vector2(-1 * Mathf.Sign(localScaleX) * Math.Abs(localScaleX), localScaleY);

        MoveForward();
    }

    protected virtual void KnockbackPlayer()
    {
        var playerRigidBody = player.GetComponent<Rigidbody2D>();

        var xDistance = transform.position.x - player.transform.position.x;

        if (xDistance >= 0)
        {
            //Knock back player left
            playerRigidBody.AddForce(Vector2.left * knockbackAmount);
        }
        else
        {
            playerRigidBody.AddForce(Vector2.right * knockbackAmount);
        }

        if (knockbackSound)
        {
            AudioSource.PlayClipAtPoint(knockbackSound, transform.position);
        }
    }

    protected void DamagePlayer()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<HealthIndicator>().DecreaseHealth(knockBackDamage);
    }


    protected void SetFade()
    {
        isFading = true;
    }

    protected void FadeAway()
    {
        var material = GetComponent<Renderer>().material;
        var materialColor = material.color;
        material.color = new Color(materialColor.r, materialColor.g, materialColor.b, materialColor.a - (fadePerSecond * Time.deltaTime));


        // Call destroy if alpha = 0;

        if (materialColor.a <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected void unHurt()
    {
        isHurt = false;
        animator.SetBool("Hurt", false);
    }

    protected void colorHurt()
    {
        spriteRenderer.color = hurtColor;
    }

    protected void colorNormal()
    {
        spriteRenderer.color = defaultColor;
    }

    protected void ShowDeathParticles()
    {
        if (deathParticles)
        {
            var particlePosition = GetDeathParticlePosition();

            deathParticlesLocal = Instantiate(deathParticles, particlePosition, Quaternion.identity);
            deathParticlesLocal.transform.parent = transform;
            deathParticlesLocal.transform.localScale = new Vector3(1f, 1f, 1f);
            deathParticlesShown = true;
        }
    }
 

    protected Vector2 GetDeathParticlePosition()
    {
        var particlePosition = new Vector2(transform.position.x, transform.position.y + deathParticleYOffset);
        return particlePosition;
    }

    protected virtual void Hurt(float damage = 0f)
    {

        var currentXVelocity = rigidBody2D.velocity.x;
        var currentYVelocity = rigidBody2D.velocity.y;


        if (canBeStunned)
        {
            float afterCollisionVelocityX = afterCollisionSpeed * Math.Sign(currentXVelocity);

            rigidBody2D.velocity = new Vector2(afterCollisionVelocityX, currentYVelocity);

            

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

    protected bool setToDie()
    {
        return (Health <= 0);
    }

    protected void PlayDeathSound()
    {
        if (deathSound)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }
    }

    protected virtual void Die()
    {
        isDying = true;

        gameObject.layer = LayerMask.NameToLayer("Background");

        //Show death animation
        animator.SetBool("Walking", false);
        animator.SetBool("Hurt", false);
        animator.SetBool("Death", true);

        //Show particles
        ShowDeathParticles();

        Invoke("PlayDeathSound", .4f);
        Invoke("SetFade", .4f);
    }


    protected int GetPlayerForegroundLayerMask()
    {
        //Convert Layer Name to Layer Number
        int invisLayerIndex = LayerMask.NameToLayer("Invisible");
        //int backgroundLayerIndex = LayerMask.NameToLayer("Background");

        int playerLayerIndex = LayerMask.NameToLayer("Player");
        int groundLayerIndex = LayerMask.NameToLayer("Foreground");
        int breakableLayerIndex = LayerMask.NameToLayer("Breakables");

        //int layerMask = (1 << cubeLayerIndex) | (1 << sphereLayerIndex);
        //int layerMask = ~((1 << invisLayerIndex) | (1 << backgroundLayerIndex));
        int layerMask = (1 << playerLayerIndex) | (1 << groundLayerIndex) | (1 << breakableLayerIndex);
        return layerMask;
    }

    protected bool playerIsObstructed()
    {
        var center = transform.GetComponent<Renderer>().bounds.center;
        //Get layer mask
        int layerMask = GetPlayerForegroundLayerMask();

        var hit = Physics2D.Linecast(center, player.transform.position, layerMask);

        if (hit != null)
        {
            if (hit.collider != null)
            {
                //Debug.Log("Raycast hit collision! - " + hit.collider.name);
                var name = hit.collider.name;
                if (name != playerName)
                {
                    return true;
                }
            }
        }

        return false;
    }


    protected bool playerIsInRange()
    {
        var distance = Vector2.Distance(transform.position, player.transform.position);

        bool inRange = (distance <= playerDetectionDistance);
        return inRange;
    }

    protected bool canSeePlayer()
    {
        if (!playerIsInRange())
        {
            return false;
        }

        //Debug.Log("canSeePlayer: In range");

        var xOffset = 0.5f;
        if (!isFacingRight())
        {
            xOffset = -xOffset;
        }

        var yOffset = 0.5f;
        var center = transform.GetComponent<Renderer>().bounds.center;

        var startPoint = center + new Vector3(xOffset, yOffset, 0);

        //Get layer mask
        int layerMask = GetPlayerForegroundLayerMask();

        var hit = Physics2D.Linecast(startPoint, player.transform.position, layerMask);

        if (hit != null)
        {
            if (drawDebugSightLines)
            {
                Debug.DrawLine(startPoint, hit.point, Color.green, 2f, false);
            }
         
            if (hit.collider != null)
            {
                //Debug.Log("Raycast hit collision! - " + hit.collider.name);
                var name = hit.collider.name;
                if (name == playerName)
                {
                    return true;
                }
            }
        }

        return false;
    }
    protected bool CloseToJumpEdge()
    {
        var center = transform.GetComponent<Renderer>().bounds.center;


        int invisLayerIndex = LayerMask.NameToLayer("Invisible");
        int layerMask = (1 << invisLayerIndex);

        var direction = Vector2.left;
        if (isFacingRight())
        {
            direction = Vector2.right;
        }

        var hit = Physics2D.Raycast(center, direction, edgeDetectionDistance, layerMask);

        if (hit)
        {
            Debug.DrawLine(center, hit.point, Color.red, 2f, false);
            if (hit.collider != null)
            {
                // Debug.Log("Raycast-edge hit collision! - " + hit.collider.tag);
                var tag = hit.collider.tag;
                if (tag == "JumpEdge")
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected bool CloseToEdge()
    {
        var center = transform.GetComponent<Renderer>().bounds.center;


        int invisLayerIndex = LayerMask.NameToLayer("Invisible");
        int layerMask = (1 << invisLayerIndex);

        var direction = Vector2.left;
        if (isFacingRight())
        {
            direction = Vector2.right;
        }

        var hit = Physics2D.Raycast(center, direction, edgeDetectionDistance, layerMask);

        if (hit)
        {
            Debug.DrawLine(center, hit.point, Color.red, 2f, false);
            if (hit.collider != null)
            {
                // Debug.Log("Raycast-edge hit collision! - " + hit.collider.tag);
                var tag = hit.collider.tag;
                if (tag == "JumpEdge" || tag == "Wall")
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected void HandlePlayerCollision(Collision2D collision)
    {
        var name = collision.collider.name;
        if (name == playerName && !isColliding)
        //if (name == playerName && collision.collider.GetType() == typeof(CapsuleCollider2D))
        {
            isColliding = true;
            Debug.Log("Collision with Xin");
            var currentXVelocity = rigidBody2D.velocity.x;
            var currentYVelocity = rigidBody2D.velocity.y;
            //Debug.Log("Xin collision velocity " + currentXVelocity);

            //Knockback and damage Xin
            if (!player.isHurt && !player.isDying && !player.isRespawning)
            {
                //player.isHurt = true;
                KnockbackPlayer();
                DamagePlayer();
            }
            isColliding = false;
        }
    }

    public void SetHealth(float hp)
    {
        Health = hp;
    }

}
