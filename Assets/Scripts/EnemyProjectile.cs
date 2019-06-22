using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {

	[SerializeField] int enemyDamage;
	//[SerializeField] float appliedForce;
	Vector2 moveVec;

	private Collider2D coll;
	private GameObject player;

	public bool knockbackToggle = false;
	public float knockbackAmount = 1000f;

	// Use this for initialization
	void Start () {
		coll = GetComponent<Collider2D>();
		player = GameObject.FindGameObjectWithTag("Player");
		//Vector2 moveVec = new Vector2(appliedForce, 0);
	}
	
	// Damage the player on collision.
	private void OnCollisionEnter2D(Collision2D collision)
	{
        //Debug.Log("Enemy projectile: OnCollisionEnter2D");
        if (collision.collider.tag == "Player") {

            collision.collider.GetComponent<HealthIndicator>().DecreaseHealth(1);
            var player = collision.collider.GetComponent<PlayerControl>();
            player.isHurt = true;
            player.xinHurtEffect();

			if (knockbackToggle == true) {
				KnockbackPlayer();
			}

		}
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enemy projectile: OnTriggerEnter2D");
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
