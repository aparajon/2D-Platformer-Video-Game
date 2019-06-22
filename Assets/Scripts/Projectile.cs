using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {


    [SerializeField] float damage = 100f;
    private bool hasCollided = false;

    public float GetDamage()
    {
        return damage;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.hasCollided == true) { return; }
        this.hasCollided = true;

		Destroy(gameObject);
    }
}
