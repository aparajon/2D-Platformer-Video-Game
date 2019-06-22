using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerControl : MonoBehaviour
{

    Animator animator;
    AnimationEvent evt;
    Rigidbody2D playerRigidBody;
    CapsuleCollider2D capCollider;
    BoxCollider2D feetCollider;
    WeaponManager weaponManager;

    [SerializeField] float runSpeed = 8f;
    [SerializeField] float jumpSpeed = 30f;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip hurtSound;
    [SerializeField] AudioClip runSound;
    [SerializeField] AudioClip deathSound;




    public int canMove;

    //projectile animation 
    public GameObject projectile;
    public float projVelocity;
    //cooldown timers
    bool canShoot = true;
    private float startTime = 0f;
    private float coolDown = .3f;

    //death timers
    bool isDying = false;
    private float timeOfDeath = 0f;
    private float deathTimer = .15f;

    //health values
    public int oldHealth;
    public int currentHealth;

    //respawn
    private Vector2 startPosition;

    // Use this for initialization
    void Start()
    {
        canMove = 1;
        projVelocity = 3000;
        startPosition = transform.position;
        animator = GetComponent<Animator>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        capCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
        weaponManager = GetComponent<WeaponManager>();
        oldHealth = GetComponent<HealthIndicator>().health;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(canMove);
        if (canMove >= 1 && !isDying)
        {
            Run();
            Jump();
            Shoot();
            Attack();
            FlipSprite();
            cooldownTimer();
        }
        checkHealth();
        checkDeath();
    }

    // On attack animations or other animations where input should be stopped set this to
    // zero using animation flags.
    public void CanMove(int type)
    {
        // Set 1 for true, 0 for false.
        canMove = type;

        // If canMove is false, stop the player's horizontal movement gradually.
        if (canMove < 1)
        {
            playerRigidBody.velocity = new Vector2(0, 0);
        }
    }

    private void Run()
    {
        // Check if player has horizontal speed.  If so, change to running animation.
        bool playerHasHorizontalSpeed = Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon;
        bool playerInput;
        if (CrossPlatformInputManager.GetAxis("Horizontal") != 0)
        {
            playerInput = true;
        }
        else
        {
            playerInput = false;
        }
        if (playerHasHorizontalSpeed && playerInput)
        {
            //AudioSource.PlayClipAtPoint(runSound, transform.position); //sound audio file play
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        // Set player velocity on horizontal move.
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // value is -1 to +1
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, playerRigidBody.velocity.y);
        playerRigidBody.velocity = playerVelocity;
    }

    private void Jump()
    {
        // Check that jumping only occurs if the player is touching the Foreground layer.
        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Foreground"))) { return; }

        // If Jump button pressed, player jumps.
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            playerRigidBody.velocity += jumpVelocityToAdd;
            AudioSource.PlayClipAtPoint(jumpSound, transform.position, 0.1f);

        }
    }

    // currently hitObjects is set to [0], which detects the player's own collider. Used to test health decrease.
    private void Attack()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            // animator.SetTrigger("isSlash");
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, 1.0f);
            //hitObjects[0].SendMessage("DecreaseHealth", 1);
            Debug.Log("Hit" + hitObjects[0].name);
        }
    }

    private void FlipSprite()
    {
        // If player as a horizontal velocity, flip their sprite.
        bool playerHasHorizontalSpeed = Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(playerRigidBody.velocity.x), 1f);
        }
    }

    private void Shoot()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon;
        if (CrossPlatformInputManager.GetButtonDown("Fire1") && canShoot)
        {
            if (playerHasHorizontalSpeed)
            {
                animator.SetTrigger("isRunShooting");
            }
            else
            {
                animator.SetTrigger("isStandShooting");
            }
            fireProjectile();
			GameObject.FindGameObjectWithTag("Sandra").GetComponent<Animator>().SetTrigger("isFiredSandra");
			canShoot = false;
        }
    }

    private void fireProjectile()
    {
        GameObject arrow = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
        AudioSource.PlayClipAtPoint(shootSound, transform.position);
        if (transform.localScale.x > 0)
        {
            arrow.GetComponent<Rigidbody2D>().AddForce(transform.right * 1000);
        }
        else
        {
            arrow.GetComponent<Rigidbody2D>().AddForce(-transform.right * 1000);
        }
        Destroy(arrow, .75f);
    }
    //sets a timer for how long in between each bullet
    private void cooldownTimer()
    {
        if (startTime < coolDown)
        {
            startTime += .01f;
        }
        else
        {
            canShoot = true;
            startTime = 0;
        }
    }
    //gives two seconds to load before we respawn
    private void deathClock()
    {
        if (timeOfDeath < deathTimer)
        {
            timeOfDeath += .01f;
        }
        else
        {
            isDying = false;
            timeOfDeath = 0f;
            Respawn();
        }
    }


    private void checkDeath()
    {
        if (GetComponent<HealthIndicator>().health <= 0 && isDying == false)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position); //plays upon death
            isDying = true;
            animator.SetTrigger("isDead");

        }
        if (isDying == true)
        {
            CanMove(0);
            deathClock();
        }
    }

    private void Respawn()
    {
        CanMove(1);
        transform.position = startPosition;
        animator.SetTrigger("isIdle");
        GetComponent<HealthIndicator>().health = 6;

        var enemyLevelController = FindObjectOfType<EnemyLevelController>();
        if (enemyLevelController)
        {
            enemyLevelController.ResetEnemies();
        }
    }

    private void checkHealth()
    {
        currentHealth = GetComponent<HealthIndicator>().health;
        {
        if (currentHealth < oldHealth)
            animator.SetTrigger("isHurt");
            oldHealth = currentHealth;
        }
    }

}