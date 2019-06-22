using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDeath : MonoBehaviour {

	private HealthIndicator healthIndicator;

	private void Start()
	{
		healthIndicator = FindObjectOfType<HealthIndicator>();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.collider.tag == "Player") {
			healthIndicator.DecreaseHealth(20);
		}
		else {
			Destroy(collision.gameObject);
		}
	}

	
}
