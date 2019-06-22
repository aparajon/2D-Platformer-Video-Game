using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchScript : MonoBehaviour {

    protected Rigidbody2D rigidBody2D;
    private bool isTriggered;
    protected SpriteRenderer spriteRenderer;

    [SerializeField] protected AudioClip hitSound;
    [SerializeField] SpriteRenderer triggeredSprite;

    // Use this for initialization
    void Start () {
        isTriggered = false;
        rigidBody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update () {	
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var projectile = collision.gameObject.GetComponent<Projectile>();
        if (projectile && isTriggered == false)
        {
            isTriggered = true;
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
            spriteRenderer.sprite = triggeredSprite.sprite;
            breakGate();
        }
    }

    private void breakGate()
    {
        GameObject gate = transform.parent.gameObject;
        transform.parent = null;
        Destroy(gate);
    }


}
