using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollClouds : MonoBehaviour {

	Vector2 orgPos;
	public float resetPos = 280;

	// Use this for initialization
	void Start () {
		orgPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		MoveClouds();
	}

	private void MoveClouds()
	{
		if (transform.position.x > resetPos) {
			transform.position = new Vector2(transform.position.x - 0.02f, orgPos.y);
		} else {
			transform.position = orgPos;
		}
	}
}
