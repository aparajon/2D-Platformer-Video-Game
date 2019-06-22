using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TombstoneBehavior : BreakableBehavior {

	// Use this for initialization
	// Update is called once per frame
	void Update () {
        breakObject();
        changeSprite();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var projectile = collision.gameObject.GetComponent<Projectile>();
        if (projectile)
        {
            float damage = projectile.GetDamage();
            Health -= damage;
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }
    }


}
