using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBehavior : MonoBehaviour {

    [SerializeField] float Health = 600f;
    [SerializeField] float baseMoveSpeed = 1f;
	[SerializeField] bool startsRight = true;
	[SerializeField] GameObject projectile;
	
	[SerializeField] float projectileSpeedX;
	[SerializeField] float playerDetectionDistance = 8f;
	[SerializeField] float projectileRate = 1f;
    [SerializeField] float edgeDetectionDistance;
	[SerializeField] float alertTimer = 15f;
	[SerializeField] float alertImageDuration = 5f;
	[SerializeField] float fadePerSecond = 2.5f;
    [SerializeField] float attackPause = 1.5f;
    [SerializeField] AudioClip projectileSound;
	[SerializeField] AudioClip alertSound;
	[SerializeField] AudioClip hurtSound;
	[SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip knockbackSound;

    [SerializeField] GameObject deathParticles;
	[SerializeField] GameObject alertImage;
 

	[SerializeField] bool attackMode = false; // currently not in use

	Rigidbody2D rigidBody2D;
	Animator animator;
	PlayerControl player;
	SpriteRenderer spriteRenderer;

	private float currentMoveSpeed;
	private float nextFire = 0.0f;

	public bool isAttacking = false;
	public bool isAlerted = false;
	private bool isHurt = false;
	private bool deathParticlesShown = false;
    private bool showAlertImage = false;
    private float lastAlertTime = 0.0f;
	private float lastAttackTime = 0.0f;
	private float lastHurtTime = 0.0f;
    private float lastSeenPlayerTime = 0.0f;


	private float alertOffset = 1.75f;

	public bool isDying = false;
	public bool isFading = false;


	private GameObject deathParticlesLocal;
    private GameObject alertLocal;

	// Use this for initialization
	void Start () {
		rigidBody2D = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		player = FindObjectOfType<PlayerControl>();
		currentMoveSpeed = baseMoveSpeed;
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

        var alertPosition = new Vector2(transform.position.x, transform.position.y + alertOffset);
        alertLocal = Instantiate(alertImage, alertPosition, Quaternion.identity);
        alertLocal.transform.parent = transform;
        alertLocal.transform.localScale = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {

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

			if (isFacingPlayer() && canAttackPlayer())
			{
				Alert(true);
			}

			else
			{
				//MoveAlongPlatform();
				MoveForward();
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


			// Attack if player is in range
			if (canAttackPlayer())
			{
                lastSeenPlayerTime = currentTime;
                if (currentTime > nextFire)
				{
					nextFire = currentTime + projectileRate;
					Attack();
				}
			}

			else
			{
                var lastSeenPlayerDiff = currentTime - lastSeenPlayerTime;
                Debug.Log("Last seen player: " + lastSeenPlayerDiff);
                if (lastSeenPlayerDiff > attackPause)
                {
                    MoveAlongPlatform();
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

	private void CheckUnalert()
	{
		var currentTime = Time.time;
		// Check if enemy should be un-alerted by checking last attack and hurt time
		// A high value in either means player is out of range for extended time

		var attackTimeDiff = currentTime - lastAttackTime;
		var hurtTimeDiff = currentTime - lastHurtTime;
		if (attackTimeDiff >= alertTimer)
		{
			// Debug.Log("Last hurt time " + lastHurtTime + " hurt time diff: " + hurtTimeDiff);
			if (hurtTimeDiff == 0f || hurtTimeDiff >= alertTimer)
			{
				Debug.Log("Enemy out of range for extended time period.");
				Unalert();
			}
		}
	}

	bool isFacingPlayer()
	{
		var xDistance = transform.position.x - player.transform.position.x;
		return (Mathf.Sign(transform.localScale.x) == -Mathf.Sign(xDistance));
	}

	private void SetAlertImage()
	{
		var currentTime = Time.time;
		var alertTimeDiff = currentTime - lastAlertTime;

		if (isAlerted && showAlertImage && alertTimeDiff < alertImageDuration)
		{
			// Debug.Log("Alert image picture");
			var alertPosition = new Vector2(transform.position.x, transform.position.y + alertOffset);
			alertLocal.transform.position = alertPosition;
		}
	}

	private void Die()
	{
		//Debug.Log("Skeleton: Die() called");

		isDying = true;

		gameObject.layer = LayerMask.NameToLayer("Background");

		//Show death animation
		animator.SetBool("Walking", false);
		animator.SetBool("Attacking", false);
		animator.SetBool("Hurt", false);
		animator.SetBool("Death", true);

		//Show particles
		ShowDeathParticles();
		//Invoke("ShowDeathParticles", .2f);
		Invoke("PlayDeathSound", .4f);
		Invoke("SetFade", .4f);

		//Destroy(gameObject);
	}

	private void SetFade()
	{
		isFading = true;
	}

	private void FadeAway()
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

	private void FacePlayer()
	{
		var xDistance = transform.position.x - player.transform.position.x;


		var localScaleX = transform.localScale.x;
		var localScaleY = transform.localScale.y;


		transform.localScale = new Vector2(-1 * Mathf.Sign(xDistance) * Math.Abs(localScaleX), localScaleY);

		//Debug.Log("Face player - localScaleX: " + localScaleX);
	}

	private void MoveForward()
	{
		// Debug.Log("Move forward called");

		var yVelocity = rigidBody2D.velocity.y;

		float nextMoveSpeed = currentMoveSpeed;

		if (!isAlerted)
		{
			nextMoveSpeed = UnityEngine.Random.Range(0.25f, nextMoveSpeed);
		}

		//else
		//{
		//    nextMoveSpeed = UnityEngine.Random.Range(1f, nextMoveSpeed);
		//}

		

		if (isFacingRight())
		{
			rigidBody2D.velocity = new Vector2(nextMoveSpeed, yVelocity);
		}
		else
		{
			rigidBody2D.velocity = new Vector2(-nextMoveSpeed, yVelocity);
		}
		
		animator.SetBool("Walking", true);
		animator.SetBool("Attacking", false);
		isAttacking = false;
	}

	private void MoveBackward()
	{
		var localScaleX = transform.localScale.x;
		var localScaleY = transform.localScale.y;
		//transform.localScale = new Vector2(-Mathf.Sign(transform.localScale.x) * localScaleX, localScaleY);
		transform.localScale = new Vector2(-1 * Mathf.Sign(localScaleX) * Math.Abs(localScaleX), localScaleY);
		// Debug.Log("MoveBackward - new localscaleX: " + transform.localScale.x);

		MoveForward();
	}

	private bool playerIsInRange()
	{
		var distance = Vector2.Distance(transform.position, player.transform.position);

		bool inRange = (distance <= playerDetectionDistance);
		return inRange;
	}

	private bool canAttackPlayer()
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
		int layerMask = GetLayerMask();

		var hit = Physics2D.Linecast(startPoint, player.transform.position, layerMask);

		if (hit != null)
		{
			 Debug.DrawLine(startPoint, hit.point, Color.green, 2f, false);
			if (hit.collider != null)
			{
				//Debug.Log("Raycast hit collision! - " + hit.collider.name);
				var name = hit.collider.name;
				if (name == "Xin")
				{
					return true;
				}
			}
		}

		return false;

	}

	private bool playerIsObstructed()
	{
		var center = transform.GetComponent<Renderer>().bounds.center;
		//Get layer mask
		int layerMask = GetLayerMask();

		var hit = Physics2D.Linecast(center, player.transform.position, layerMask);

		if (hit != null)
		{
			if (hit.collider != null)
			{
				//Debug.Log("Raycast hit collision! - " + hit.collider.name);
				var name = hit.collider.name;
				if (name != "Xin")
				{
					return true;
				}
			}
		}

		return false;
	}

	private int GetLayerMask()
	{
		//Convert Layer Name to Layer Number
		int invisLayerIndex = LayerMask.NameToLayer("Invisible");
		//int backgroundLayerIndex = LayerMask.NameToLayer("Background");

		int playerLayerIndex = LayerMask.NameToLayer("Player");
		int groundLayerIndex = LayerMask.NameToLayer("Foreground");

		//int layerMask = (1 << cubeLayerIndex) | (1 << sphereLayerIndex);
		//int layerMask = ~((1 << invisLayerIndex) | (1 << backgroundLayerIndex));
		int layerMask = (1 << playerLayerIndex) | (1 << groundLayerIndex);
		return layerMask;
	}

	void Attack()
	{
        rigidBody2D.velocity = new Vector2(0, 0);

        if (boneVelocityTooHigh())
        {
            return;
        }

        animator.SetBool("Walking", false);
		animator.SetBool("Attacking", true);
		isAttacking = true;

		//spawn projectile
		ThrowBone();
		lastAttackTime = Time.time;

		Invoke("StopAttackAnimation", 0.5f);

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
		Debug.Log("Skeleton: Throw bone called.");
		//start position front of character
		float xOffset = .75f * Mathf.Sign(transform.localScale.x);
		//Debug.Log("xOffset: " + xOffset);

        // Debug.Log("xVelocity: " + xVelocity + " yVelocity: " + yVelocity);

        Vector3 boneOffSet = transform.position + new Vector3(xOffset, 1.25f, 0);


        //throw in direction facing
        float xVelocity = projectileSpeedX * Mathf.Sign(transform.localScale.x);
        float yVelocity = CalculateVelocityY(boneOffSet, player.transform.position, xVelocity);

        if (boneVelocityTooHigh())
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
	}

    private bool boneVelocityTooHigh()
    {
        float xOffset = .75f * Mathf.Sign(transform.localScale.x);
        Vector3 boneOffSet = transform.position + new Vector3(xOffset, 1.25f, 0);
        float xVelocity = projectileSpeedX * Mathf.Sign(transform.localScale.x);
        float yVelocity = CalculateVelocityY(boneOffSet, player.transform.position, xVelocity);
        if (yVelocity > 15)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

	void StopAttackAnimation()
	{
		animator.SetBool("Walking", true);
		animator.SetBool("Attacking", false);
	}

	void StopAttack()
	{
		animator.SetBool("Walking", true);
		animator.SetBool("Attacking", false);
		isAttacking = false;
	}

	private bool isFacingRight()
	{
		return transform.localScale.x > 0;
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
		float yDistance = target.y-source.y;
		

		float yVelocity = (yDistance - (0.5f * g * time * time)) / time;

		// Debug.Log("xDistance: " + xDistance + " yDistance: " + yDistance + " time: " + time);

		return yVelocity;
	}

	private void Alert(bool effects = false)
	{
        Debug.Log("Alert called.");
      
		isAlerted = true;
       
      
		currentMoveSpeed = 2 * baseMoveSpeed;

		lastAlertTime = Time.time;

		if (effects)
		{
            ShowAlert();
            Invoke("HideAlert", alertImageDuration);

            if (alertSound)
			{
				AudioSource.PlayClipAtPoint(alertSound, transform.position);
			}
		}
		
	}

	private void Unalert()
	{
		isAlerted = false;
        HideAlert();
		currentMoveSpeed = baseMoveSpeed;
	}

	void ShowDeathParticles()
	{
		if (deathParticles)
		{

			var particlePosition = GetDeathParticlePosition();

			deathParticlesLocal = Instantiate(deathParticles, particlePosition, Quaternion.identity);
			deathParticlesLocal.transform.parent = transform;
			deathParticlesLocal.transform.localScale = new Vector3(1f, 1f, 1f);
			deathParticlesShown = true;
			//var ps = particles.GetComponent<ParticleSystem>();
			//ps.GetComponent<Renderer>().sortingLayerName = "Effects";
		}
	}

	private Vector2 GetDeathParticlePosition()
	{
		var particleOffsetY = 1f;
		var particlePosition = new Vector2(transform.position.x, transform.position.y + particleOffsetY);
		return particlePosition;
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
			var name = collision.collider.name;

			if (name == "Xin")
			{
				Debug.Log("Collision with Xin");
				var currentXVelocity = rigidBody2D.velocity.x;
				var currentYVelocity = rigidBody2D.velocity.y;
				//Debug.Log("Xin collision velocity " + currentXVelocity);
              
                //Knockback and damage Xin
                KnockbackPlayer();

			
			}
		}
	}

	private void KnockbackPlayer()
	{
        Debug.Log("Knockback called");
        var playerRigidBody = player.GetComponent<Rigidbody2D>();

		var xDistance = transform.position.x - player.transform.position.x;
		//Debug.Log("Knockback: xDistance: " + xDistance);


		var playerBeforeVelocityX = playerRigidBody.velocity.x;
		var playerBeforeVelocityY = playerRigidBody.velocity.y;

		//Debug.Log("Player before velocity x: " + playerBeforeVelocityX + " y: " + playerBeforeVelocityY);
        
        if (xDistance >= 0)
        {
            //Knock back player left
            playerRigidBody.AddForce(Vector2.left * 10000f);
        }
        else
        {
            playerRigidBody.AddForce(Vector2.right * 10000f);
        }

        if (knockbackSound)
        {
            AudioSource.PlayClipAtPoint(knockbackSound, transform.position);
        }

        GetComponent<HealthIndicator>().health -= 1;
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		//rigidBody2D.isKinematic = false;
	}

	private void Hurt(float damage = 0f)
	{
		if (!isAlerted)
		{
			Alert(false);
		}

		var currentXVelocity = rigidBody2D.velocity.x;
		var currentYVelocity = rigidBody2D.velocity.y;
		//Debug.Log("Hurt velocity " + currentXVelocity);

		float afterCollisionVelocityX = 2f * Math.Sign(currentXVelocity);

		rigidBody2D.velocity = new Vector2(afterCollisionVelocityX, currentYVelocity);


		isHurt = true;
		lastHurtTime = Time.time;
		//Debug.Log("Is hurt");
		animator.SetBool("Hurt", true);

		Health -= damage;

		colorHurt();
		if (hurtSound)
		{
			AudioSource.PlayClipAtPoint(hurtSound, transform.position);
		}
	  

		Invoke("colorNormal", .15f);
		Invoke("unHurt", 0.75f);

	}

	private void unHurt()
	{
		isHurt = false;
		animator.SetBool("Hurt", false);
	}

	private void colorHurt()
	{
		spriteRenderer.color = new Color32(233, 145, 145, 255);
	}

	private void colorNormal()
	{
		spriteRenderer.color = new Color32(233, 255, 255, 255);
	}

	private bool setToDie()
	{
		return (Health <= 0);
	}

	private void PlayDeathSound()
	{
		AudioSource.PlayClipAtPoint(deathSound, transform.position);
	}

	private bool CloseToJumpEdge()
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

    private void ShowAlert()
    {
        showAlertImage = true;
        alertLocal.transform.localScale = new Vector3(1, 1, 1);
    }

    private void HideAlert()
    {
        showAlertImage = false;
        alertLocal.transform.localScale = new Vector3(0, 0, 0);
    }
}
