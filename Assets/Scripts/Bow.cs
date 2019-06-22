using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class Bow : MonoBehaviour
{
    public GameObject projectile;
    public Vector2 projVelocity; 
    public float maxSpeed = 25f;
    public Rigidbody2D arrow;
    public Vector2 offset = new Vector2(0.4f, 0.1f);
    public float cooldown = 1f;


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
    }

    public void Shoot()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            GameObject go = (GameObject)Instantiate(projectile, (Vector2)transform.position + offset * transform.localScale.x, Quaternion.identity);
            go.GetComponent<Rigidbody2D>().velocity = new Vector2(projVelocity.x * transform.localScale.x, projVelocity.y);
            GetComponent<Animator>().SetTrigger("shoot");
        }

    }
}
