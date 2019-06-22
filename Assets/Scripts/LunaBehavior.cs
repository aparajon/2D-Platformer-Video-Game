using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LunaBehavior : MonoBehaviour
{
	public bool attackSkipToggle = false;

	private void Start()
	{
		InitializeComponents();
		FacePlayer();
	}

	private void Update()
	{

		InitializeHealth();

		LoadSceneOnDeath();
		UpdateHealth();
		CheckLunaDied();
		if (attackSkipToggle) {
			StartCoroutine(AttackRandomizer());
		}
	}

	/*************************************************** SET LUNA COMPONENTS ************************/
	private Rigidbody2D rb;
	private CapsuleCollider2D capsule;
	private Animator animator;
	private GameObject player;
	private SpriteRenderer spriteRenderer;
	private Material material;
	private BossLevelScript bossLevelScript;
	private AudioSource audioSource;
	private DialogManager dialogManager;

	[SerializeField] AudioClip lunaHit;
	[SerializeField] AudioClip lunaFlames;
	[SerializeField] AudioClip lunaFireball;
	[SerializeField] AudioClip lunaGuard;
	[SerializeField] AudioClip lunaAttacking;
	[SerializeField] AudioClip lunaAttacking2;

	private void InitializeComponents()
	{
		rb = GetComponent<Rigidbody2D>();
		capsule = GetComponent<CapsuleCollider2D>();
		animator = GetComponent<Animator>();
		player = GameObject.Find("Xin");
		spriteRenderer = GetComponent<SpriteRenderer>();
		bossLevelScript = GameObject.Find("Level").GetComponent<BossLevelScript>();
		material = spriteRenderer.material;
		audioSource = GameObject.Find("Music").GetComponentInChildren<AudioSource>();
		dialogManager = GameObject.Find("User Interface").GetComponentInChildren<DialogManager>();

		deathMessage = false;
		guarding = false;
		lunaDied = false;
	}

	/*************************************************** BEGIN HEALTH MANIPULATION FUNCTIONS ********/
	// Set these Sprites to heart icons.
	public Sprite fullHeart;
	public Sprite halfHeart;
	public Sprite emptyHeart;
	private bool deathMessage;
	private bool guarding;
	public bool lunaDied;

	private Image[] hearts = new Image[8];

	public int health = 16;

	private void UpdateHealth()
	{

		// Check to make sure health cannot go above number of total collected hearts.
		if (health > 16) { health = 16; }

		if (bossLevelScript.startBattleToggle == 1) {
			// Draw in a full, half, or empty heart depending on health of player.
			for (int i = 0; i < hearts.Length; i++) {
				if (i * 2 < health) {
					if (( i - health / 2 ) == 0) {
						hearts[i].sprite = halfHeart;
					}
					else {
						hearts[i].sprite = fullHeart;
					}
				}
				else {
					hearts[i].sprite = emptyHeart;
				}
			}
		}

	}

	private void InitializeHealth()
	{
		GameObject tempCanvas = GameObject.Find("User Interface");
		if (tempCanvas) {
			Image[] canvasImages = tempCanvas.GetComponentsInChildren<Image>();

			// Place all UI Heart images into hearts[].
			int heartsIterator = 0;
			for (int i = 0; i < canvasImages.Length; i++) {
				if (canvasImages[i].name == "LunaHeart(Clone)") {
					hearts[heartsIterator] = canvasImages[i];
					heartsIterator++;
				}
			}
		}
		else {
			Debug.Log("Could not find User Interface.  Please place User Interface prefab into scene.");
		}
	}

	// Decrease Luna's health by one on arrow hit.
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.collider.tag == "PlayerAttack") {
			if (guarding == false) {  // case that Luna is able to take damage.
				DecreaseHealth(1);
				AudioSource.PlayClipAtPoint(lunaHit, transform.position);
				StartCoroutine(TurnRedOnHit(0.15f));
			}
			else {  // case that Luna is guarding = increase her health by one.
				IncreaseHealth(1);
				AudioSource.PlayClipAtPoint(lunaGuard, transform.position);
				StartCoroutine(TurnBlueOnGuard(0.15f));
				ReflectArrow();
			}

		}
		else if (collision.collider.tag == "Player") {
			KnockbackPlayer();
			player.SendMessage("DecreaseHealth", 1);
		}
	}

	private void CheckLunaDied()
	{
		if (health <= 0) {
			lunaDied = true;
			animator.SetBool("LunaDied", true);
		}
		else {
			lunaDied = false;
		}
	}


	public void SetDeathMessage()
	{
		deathMessage = !deathMessage;
	}

	private void LoadSceneOnDeath()
	{
		if (deathMessage) {
			SetDeathMessage();
			Debug.Log("InDeathMessage");
			bossLevelScript.DeleteLunaUI();
			audioSource.Stop();
			dialogManager.SetStillPlaying(true);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

		}
	}

	private IEnumerator TurnRedOnHit(float time)
	{
		spriteRenderer.color = new Color32(233, 145, 145, 125);
		gameObject.layer = LayerMask.NameToLayer("Invulnerable");

		yield return new WaitForSeconds(time);

		spriteRenderer.color = new Color32(233, 255, 255, 125);

		yield return new WaitForSeconds(0.75f);
		gameObject.layer = LayerMask.NameToLayer("Enemies");
		spriteRenderer.color = new Color32(233, 255, 255, 255);

	}

	private IEnumerator TurnBlueOnGuard(float time)
	{
		spriteRenderer.color = new Color32(0, 0, 255, 255);

		yield return new WaitForSeconds(time);

		spriteRenderer.color = new Color32(233, 255, 255, 255);

	}

	public int GetHealth()
	{
		return health;
	}

	public void SetHealth(int setTo)
	{
		health = setTo;
	}

	public void DecreaseHealth(int damage)
	{
		health -= damage;
	}

	public void IncreaseHealth(int amount)
	{
		health += amount;
	}

	/*************************************************** BEGIN MOVE and ATTACK SELECTOR FUNCTIONS ***************/

	public float lerpDistance = 0.1f;
	private Vector2 smoothVelocity = Vector2.zero;
	private int randomAttack = 1;
	private Vector2 playerLastPosition;
	private Vector2 lunaLastPosition;

	private bool canMove = false;
	private bool trackPlayer = false;
	private bool getNewRandomAttack = true;
	private int doProjectile = 0;
	private bool doMelee = false;

	// Luna has multiple attacks.  Use System.Random() to generate a random number per frame she is able to
	// do a new attack.  This RN then determines the next attack.
	IEnumerator AttackRandomizer()
	{
		if (getNewRandomAttack == true) {
			playerLastPosition.x = player.transform.position.x;
			playerLastPosition.y = -4.8f;

			System.Random rng = new System.Random();
			randomAttack = rng.Next(0,8);

			getNewRandomAttack = false; // stop updates which might activate new attack.
			doProjectile = 0;
			doMelee = false;
		}
		else if (canMove == true) {
			animator.SetBool("LunaRunning", true);

			if (trackPlayer == true) {
				playerLastPosition.x = player.transform.position.x;
				lunaLastPosition = transform.position;
			}
			MoveToPlayerPosition(playerLastPosition, lunaLastPosition);
			yield return null;
		}
		else {
			// Idle for one second.
			attackSkipToggle = false;
			if (randomAttack == 0) {
				Debug.Log("In idle.");
				FacePlayer();
				yield return new WaitForSeconds(0.5f);
				getNewRandomAttack = true;
			}

			// Melee Attack Routine
			else if (randomAttack >= 1 && randomAttack <= 2) {
				// Move Luna
				if (doMelee == false) {
					Debug.Log("In melee sequence.");
					canMove = true;
					doMelee = true;
				}
				else {
					MeleeAttack();
					yield return new WaitForSeconds(1.5f);
					getNewRandomAttack = true;

				}
			}

			// Projectile Attack Routine
			else if (randomAttack >= 3 && randomAttack <= 4) {
				FacePlayer();
				// Throw between 3 and 5 projectiles in a row.
				if (doProjectile == 0) {
					AudioSource.PlayClipAtPoint(lunaAttacking, transform.position);
					Debug.Log("In projectile sequence.");
					animator.SetBool("LunaProjectile", true);
					yield return new WaitForSeconds(1.5f);

					System.Random rng = new System.Random();
					int numberProjectiles = rng.Next(2, 5);

					for (int i = 0; i < numberProjectiles; i++) {
						ProjectileAttack();
						yield return new WaitForSeconds(0.5f);
					}
					ProjectileAttack();
					animator.SetBool("LunaProjectile", false);

					yield return new WaitForSeconds(0.20f);
					doProjectile = 1;
				}
				// Move Luna
				else if (doProjectile == 1) {
					canMove = true;
					doProjectile = 2;
				}
				// Toggle Next Attack
				else {
					getNewRandomAttack = true;
				}
			}
			// Flames Attack Routine
			else if (randomAttack >= 5 && randomAttack <= 6) {
				Debug.Log("In flames sequence.");
				if (doProjectile == 0) {
					AudioSource.PlayClipAtPoint(lunaAttacking, transform.position);
					animator.SetBool("LunaFlames", true);

					yield return new WaitForSeconds(1f);
					FlamesAttack();

					yield return new WaitForSeconds(1f);
					animator.SetBool("LunaFlames", false);

					doProjectile = 1;
				}
				else {
					getNewRandomAttack = true;
				}
			}
			// Guarding Routine.
			else if (randomAttack >= 7 && randomAttack <= 8) {
				Debug.Log("In guard sequence.");
				animator.SetBool("LunaGuarding", true);
				yield return new WaitForSeconds(0.5f);
				guarding = true;
				yield return new WaitForSeconds(3f);
				guarding = false;
				animator.SetBool("LunaGuarding", false);
				getNewRandomAttack = true;
			}

			attackSkipToggle = true;
		}
	}

	private void MoveToPlayerPosition(Vector2 pos, Vector2 lunaPos)
	{
		float distance = Vector2.Distance(pos, transform.position);
		if (distance > 1.8f && IsFacingPlayer()) {
			//Debug.Log(distance);
			transform.position = Vector2.Lerp(transform.position, pos, lerpDistance);
		}
		else if (!IsFacingPlayer()) {
			animator.SetBool("LunaRunning", false);
			canMove = false;
			trackPlayer = false;
		}
		else {
			animator.SetBool("LunaRunning", false);
			canMove = false;
			trackPlayer = false;
		}
	}

	private bool IsFacingPlayer()
	{
		var xDistance = transform.position.x - player.transform.position.x;
		return ( Mathf.Sign(transform.localScale.x) == -Mathf.Sign(xDistance) );
	}

	private void FacePlayer()
	{
		Debug.Log("In FacePlayer");
		var xDistance = transform.position.x - player.transform.position.x;

		var localScaleX = transform.localScale.x;
		var localScaleY = transform.localScale.y;

		transform.localScale = new Vector2(-1 * Mathf.Sign(xDistance) * Math.Abs(localScaleX), localScaleY);
	}

	/*************************************************** BEGIN ATTACK FUNCTIONS *****************/

	[SerializeField] GameObject arrow;
	[SerializeField] GameObject fireball;
	[SerializeField] GameObject flames;
	[SerializeField] float fireballSpeed = 1500;
	[SerializeField] float flamesSpeed = 1700;
	[SerializeField] float arrowSpeed = 500;
	[SerializeField] float knockbackAmount = 5000f;

	private void MeleeAttack()
	{
		FacePlayer();
		animator.SetTrigger("LunaSlash");
		Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, 2.0f);
		AudioSource.PlayClipAtPoint(lunaAttacking2, transform.position);
		for (int i = 0; i < hitObjects.Length; i++) {
			if (hitObjects[i].tag == "Player" && hitObjects[i].GetType() == typeof(CapsuleCollider2D)) {
				hitObjects[i].SendMessage("DecreaseHealth", 1);
				KnockbackPlayer();
				Debug.Log("Hit " + hitObjects[0].name);
			}
		}
	}

	private void ProjectileAttack()
	{
		FacePlayer();
		Vector3 offset = new Vector3(1f * Mathf.Sign(transform.localScale.x), 1.25f, 0);
		GameObject fire = Instantiate(fireball, transform.position + offset , Quaternion.identity) as GameObject;
		AudioSource.PlayClipAtPoint(lunaFireball, transform.position);
		if (transform.localScale.x > 0) {
			fire.GetComponent<Rigidbody2D>().AddForce(transform.right * fireballSpeed);
		}
		else {
			fire.GetComponent<Rigidbody2D>().AddForce(-transform.right * fireballSpeed);
		}
		Destroy(fire, 1.5f);
	}

	private void ReflectArrow()
	{
		FacePlayer();
		FacePlayer();
		Vector3 offset = new Vector3(1f * Mathf.Sign(transform.localScale.x), 1.0f, 0);
		GameObject reflectedArrow = Instantiate(arrow, transform.position + offset, Quaternion.identity) as GameObject;
		if (transform.localScale.x > 0) {
			reflectedArrow.GetComponent<Rigidbody2D>().AddForce(transform.right * arrowSpeed);
		}
		else {
			reflectedArrow.GetComponent<Rigidbody2D>().AddForce(-transform.right * arrowSpeed);
		}
		Destroy(reflectedArrow, 1.5f);

	}

	private void FlamesAttack()
	{
		FacePlayer();
		Vector3 offset = new Vector3(0, -6.0f, 0);
		GameObject[] flame = new GameObject[15];
		AudioSource.PlayClipAtPoint(lunaFlames, transform.position);

		for (int i = 0; i < flame.Length; i++) {
			flame[i] = Instantiate(flames, player.transform.position + offset, Quaternion.identity) as GameObject;
			offset = offset + new Vector3(0.2f * Mathf.Sign(player.transform.localScale.x), -1.2f, 0);
			flame[i].GetComponent<Rigidbody2D>().AddForce(transform.up * flamesSpeed);
		}

		for (int i = 0; i < flame.Length; i++) {
			Destroy(flame[i], 1.5f);
		}
	}


	private void KnockbackPlayer()
	{
		var playerRigidBody = player.GetComponent<Rigidbody2D>();
		var xDistance = transform.position.x - player.transform.position.x;

		if (xDistance >= 0) {
			//Knock back player left
			playerRigidBody.AddForce(Vector2.left * knockbackAmount);
		}
		else {
			playerRigidBody.AddForce(Vector2.right * knockbackAmount);
		}

		/*if (knockbackSound) {
			AudioSource.PlayClipAtPoint(knockbackSound, transform.position);
		}*/
	}
}
