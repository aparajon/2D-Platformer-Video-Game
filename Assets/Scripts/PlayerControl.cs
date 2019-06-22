using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerControl : MonoBehaviour
{

    public Animator animator;
    AnimationEvent evt;
    Rigidbody2D playerRigidBody;
    CapsuleCollider2D capCollider;
    BoxCollider2D feetCollider;
    WeaponManager weaponManager;
    SpriteRenderer spriteRenderer;
    Material material;
    float gravityScaleAtStart; //for the ladder climbing
    bool isClimbing;
    public bool godMode;

    [SerializeField] float runSpeed = 8f;
    [SerializeField] float jumpSpeed = 30f;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip hurtSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] float invulnerabilityPeriod = 2f;
    [SerializeField] float invulnerabilityTransparency = 0.5f;
    [SerializeField] float climbSpeed = 5f;

    [SerializeField] public bool canReceiveInput = true;

    public int canMove;
	bool canShoot = true;

    //projectile animation 
    public GameObject projectile;
    public float projVelocity;

    //death timers
    public bool isDying = false;
    public bool isRespawning = false;
    public bool isHurt = false;

    //respawn
    public Vector2 spawnPosition;

    private float originalAlphaValue;

	// Implementing singleton behavior.
	static bool wasCreated = false;
	private void Awake()
	{
		if (!wasCreated) {
			DontDestroyOnLoad(this.gameObject);
			wasCreated = true;
		}
		else {
			Destroy(this.gameObject);
		}
	}

	// Use this for initialization
	void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        canMove = 1;
        projVelocity = 3000;
        //spawnPosition = transform.position;
        animator = GetComponent<Animator>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        capCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
        weaponManager = GetComponent<WeaponManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = GetComponent<Renderer>().material;
        gravityScaleAtStart = playerRigidBody.gravityScale;
        originalAlphaValue = material.color.a;
        isClimbing = false;
        godMode = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canReceiveInput)
        {
            return;
        }
        
        //Debug.Log(canMove);
        if (canMove >= 1 && !isDying)
        {
            Run();
            Jump();
            Shoot();
            //Attack();
            FlipSprite();
            ClimbLadder();
            godModeTrigger();
        }
        CheckDeath();
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
        if (playerHasHorizontalSpeed && playerInput && !isClimbing)
        {
            animator.SetBool("isRunning", true);
            if (verticalVelocityCheck())
            {
                turnOnFootsteps();
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
            turnOffFootsteps();
        }

        // Set player velocity on horizontal move.
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // value is -1 to +1
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, playerRigidBody.velocity.y);
        playerRigidBody.velocity = playerVelocity;
    }

    private void Jump()
    {
        // Check that jumping only occurs if the player is touching the Foreground layer
        //(added breakable and climbing)
        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Foreground", "Breakables", "Climbing"))) { return; }

        // If Jump button pressed, player jumps.
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            turnOffFootsteps(); //mutes the footsteps audio
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            playerRigidBody.velocity += jumpVelocityToAdd;
            AudioSource.PlayClipAtPoint(jumpSound, transform.position, 0.1f);

        }
    }
    //taken from tilevania
    private void ClimbLadder()
    {
        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")) && (!godMode))
        {
            animator.SetBool("onLadder", false);
            animator.SetBool("Climbing", false);
            isClimbing = false;
            playerRigidBody.gravityScale = gravityScaleAtStart;
            return;
        }
        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Foreground")) && (!godMode))
        {
            if (CrossPlatformInputManager.GetAxis("Vertical") != 0)
            {
                animator.SetBool("onLadder", true);
                animator.SetBool("Climbing", true);
            }
            else
            {
                animator.SetBool("onLadder", true);
                animator.SetBool("Climbing", false);
            }
        }
        else
        {
            animator.SetBool("Climbing", false);
            animator.SetBool("onLadder", false);
        }

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(playerRigidBody.velocity.x, controlThrow * climbSpeed);
        playerRigidBody.velocity = climbVelocity;
        playerRigidBody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(playerRigidBody.velocity.y) > Mathf.Epsilon;
        isClimbing = true;
    }
    
    // currently hitObjects is set to [0], which detects the player's own collider. Used to test health decrease.
    /***
    private void Attack()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            // animator.SetTrigger("isSlash");
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, 1.0f);
            //hitObjects[0].SendMessage("DecreaseHealth", 1);
            Debug.Log("Hit" + hitObjects[0].name);
        }
    }***/

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
        if (isClimbing && !godMode) { return; } //can't shoot when climbing lol
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
            FireProjectile();
			GameObject.FindGameObjectWithTag("Sandra").GetComponent<Animator>().SetTrigger("isFiredSandra");
			canShoot = false;
			StartCoroutine(CanShootAgain(0.50f));
        }
    }

    private void FireProjectile()
    {
        GameObject arrow = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
        AudioSource.PlayClipAtPoint(shootSound, transform.position);
        if (transform.localScale.x > 0)
        {
            arrow.GetComponent<Rigidbody2D>().AddForce(transform.right * 1000);
        }
        else
        {
            var localScaleX = arrow.transform.localScale.x;
            var localScaleY = arrow.transform.localScale.y;

            arrow.transform.localScale = new Vector2(-1 * localScaleX, localScaleY);

            arrow.GetComponent<Rigidbody2D>().AddForce(-transform.right * 1000);
           
        }
        Destroy(arrow, .75f);
    }

    private void CheckDeath()
    {
        if (GetComponent<HealthIndicator>().GetHealth() <= 0 && isDying == false)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position); //plays upon death
            isDying = true;
        }
        if (isDying == true)
        {
			CanMove(0);
			Respawn();
		}
    }

	private void Respawn()
    {
		isDying = false;
        isRespawning = true;
        StartCoroutine(DeathRoutine());
		GetComponent<HealthIndicator>().SetHealth(( GetComponent<HealthIndicator>().GetNumberOfHearts() * 2 ));
	}

	IEnumerator DeathRoutine()
	{
		// Toggle step sound off and set player layer to BG so enemies don't connect.
		turnOffFootsteps();
		// Play death anim. and wait for 3 seconds.
		animator.SetTrigger("isDead");
		yield return new WaitForSeconds(1f);
		TurnInvulnerable(true);
		yield return new WaitForSeconds(2f);

        //Set player respawn to checkpoint
        //if (GameObject.FindGameObjectWithTag("Checkpoint"))
        //{
        //    var checkpoint = GameObject.FindGameObjectWithTag("Checkpoint");
        //    var checkpointController = checkpoint.GetComponent<Checkpoint>();

        //    if (checkpointController.checkPointReached)
        //    {
        //        transform.position = checkpoint.transform.position;
        //    }
        //}
        
        // Go back to spawn position
        transform.position = spawnPosition;
        


        // Respawn all enemies.
    
		var enemyLevelController = FindObjectOfType<EnemyLevelController>();
		if (enemyLevelController) {
			enemyLevelController.ResetEnemies();
		}

        //Reset platforms
        var platformLevelController = FindObjectOfType<PlatformLevelController>();
        if (platformLevelController)
        {
            platformLevelController.ResetPlatforms();
        }


        // IF LUNA IS IN ROOM, RELOAD LEVEL.
        if (GameObject.Find("Luna")) {
			GameObject.Find("Luna").SendMessage("SetDeathMessage");
		}

   

		// Reset player layer, idle anim.
		animator.SetTrigger("isIdle");
		yield return new WaitForSeconds(1f);
		// Let player move again.
		GetComponent<HealthIndicator>().SetHealth(( GetComponent<HealthIndicator>().GetNumberOfHearts() * 2 ));
		TurnInvulnerable(false);
		CanMove(1);
        isRespawning = false;
    }

	IEnumerator CanShootAgain(float cooldown)
	{
		yield return new WaitForSeconds(cooldown);
		canShoot = true;
	}

	//changes the volume of the audio source based off the movement of the character
	//TO-DO - might be able to stop the audio rather than change volume
    public void turnOnFootsteps()
    {
		GetComponent<AudioSource>().volume = .125f; //set volume to one AKA highest
    }

    public void turnOffFootsteps()
    {
        GetComponent<AudioSource>().volume = 0f;  //set volume to zero AKA lowest
    }
	//checks whether the rigid body is moving vertically or not, returning true if you arent vertically moving
    private bool verticalVelocityCheck()
    {
        return (Mathf.Abs(playerRigidBody.velocity.y) == 0);
    }

    //causes the damage animation to occur and the relevant damage noises to play if a collision is detected
    private void OnCollisionEnter2D(Collision2D collision)
    {
    //if either player colliders touch an enemy collider, the player flashes red for a moment and ouch noises play 
        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Enemies")) || capCollider.IsTouchingLayers(LayerMask.GetMask("Enemies"))) //feet collider
        {
            if (!isHurt)
            {
                xinHurtEffect();
            }
            

        }
        //if a enemy projectile hits Xin, same red and sound effects
        //TO-DO - troubles with the Enemy Projectile layer colliding with Xin, want to change it to previous one
        //else if (collision.collider.tag == "Enemy Projectile")
        //{
        //    if (!isHurt)
        //    {
        //        xinHurtEffect();
        //    }

        //}

        //else if (capCollider.IsTouchingLayers(LayerMask.GetMask("Enemy Projectile")))
        //{
        //    if (!isHurt)
        //    {
        //        xinHurtEffect();
        //        isHurt = true;
        //    }

        //}

  
    }

    public void xinHurtEffect()
    {
        isHurt = true;
        AudioSource.PlayClipAtPoint(hurtSound, transform.position);
        turnRed();
        Invoke("turnNormalColor", .15f); //delay of .15 associated with turning back

        StartCoroutine(TurnInvulnerable(invulnerabilityPeriod));
    }

	//edits player sprite to red hue
    private void turnRed()
    {
        spriteRenderer.color = new Color32(233, 145, 145, 255);
    }
	//edits player sprite to normal shade
    private void turnNormalColor()
    {
        spriteRenderer.color = new Color32(233, 255, 255, 255);
    }

    public IEnumerator TurnInvulnerable(float howLong)
    {
		var material = GetComponent<Renderer>().material;

		// Turn invulnerable and continue being invulnerable until invulnerability period ends.
		gameObject.layer = LayerMask.NameToLayer("Invulnerable");
        var materialColor = material.color;
        material.color = new Color(materialColor.r, materialColor.g, materialColor.b, invulnerabilityTransparency);

		// Wait for howLong number of seconds.
		yield return new WaitForSeconds(howLong);

		// End invulnerability.
		material.color = new Color(materialColor.r, materialColor.g, materialColor.b, originalAlphaValue);
		gameObject.layer = LayerMask.NameToLayer("Player");
		isHurt = false;
	}

	private void TurnInvulnerable(bool toggle)
	{
		var materialColor = material.color;
		if (toggle == true) {
			material.color = new Color(materialColor.r, materialColor.g, materialColor.b, invulnerabilityTransparency);
			gameObject.layer = LayerMask.NameToLayer("Invulnerable");
		}
		else {
			material.color = new Color(materialColor.r, materialColor.g, materialColor.b, originalAlphaValue);
			gameObject.layer = LayerMask.NameToLayer("Player");
		}
	}

    private void godModeTrigger()
    {
        godModeHeal();
        if (CrossPlatformInputManager.GetButtonDown("God"))
        {
            if (!godMode)
            {
                godMode = true;
                enterGodMode();
            }
            else
            {
                godMode = false;
                exitGodMode();
            }
        }
    }

    private void enterGodMode()
    {
        gameObject.layer = LayerMask.NameToLayer("Invulnerable");
        playerRigidBody.gravityScale = 0f;
        playerRigidBody.mass = 1000f;

    }
    private void exitGodMode()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        playerRigidBody.gravityScale = gravityScaleAtStart;
        playerRigidBody.mass = 1f;
    }
    private void godModeHeal()
    {
        if (godMode)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<HealthIndicator>().IncreaseHealth(6);
        }
    }

}