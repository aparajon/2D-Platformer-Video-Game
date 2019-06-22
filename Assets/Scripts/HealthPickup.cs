using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour {


	[SerializeField] AudioClip gotHealth;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player") {
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			player.GetComponent<HealthIndicator>().IncreaseHealth(2);
			AudioSource.PlayClipAtPoint(gotHealth, player.transform.position, 0.5f);
			Destroy(gameObject);
		}
	}
}
