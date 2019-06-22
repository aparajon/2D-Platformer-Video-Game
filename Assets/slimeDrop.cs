using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slimeDrop : MonoBehaviour {


	// Use this for initialization
	void Start () {
    }

    // Update is called once per frame
    void Update () {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Enemy projectile: OnCollisionEnter2D");
        if (collision.collider.tag == "Player")
        {
            collision.collider.GetComponent<HealthIndicator>().DecreaseHealth(1);
            var player = collision.collider.GetComponent<PlayerControl>();
            player.isHurt = true;
            player.xinHurtEffect();
        }
        Destroy(gameObject);
    }

}
