using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BreakableBehavior : MonoBehaviour {

    protected Rigidbody2D rigidBody2D;
    protected Animator animator;
    protected PlayerControl player;
    protected SpriteRenderer spriteRenderer;

    public bool isBreaking;

    [SerializeField] protected float Health;
    [SerializeField] SpriteRenderer damagedSprite;
    [SerializeField] protected AudioClip hitSound;
    [SerializeField] protected AudioClip breakSound;
    [SerializeField] protected GameObject dropOne;

    // Use this for initialization
    void Start () {
        Initialize();
    }

    // Update is called once per frame
    void Update () {
    }

    protected void Initialize()
    {
        Health = 200f;
        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerControl>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isBreaking = false;
    }

    //causes the sprite to change to the damaged version, if there is one
    public void changeSprite()
    {
        if (Health < 200f )
            spriteRenderer.sprite = damagedSprite.sprite;
    }

    //if the hp is under a certain level, check whether or not it is currently fading
    public void breakObject()
    {
        if (Health <= 0 && isBreaking == false)
        {
            isBreaking = true;
            FadeAway();
        }
        if (Health <= 0 && isBreaking == true)
        {
            FadeAway();
        }

    }
    //references EnemyBehavior.cs fade code to add a fading to breaking animation
    protected void FadeAway()
    {
        var material = GetComponent<Renderer>().material;
        var materialColor = material.color;
        material.color = new Color(materialColor.r, materialColor.g, materialColor.b, materialColor.a - (2.5f * Time.deltaTime));
        // Call destroy if alpha = 0;

        if (materialColor.a <= 0)
        {
            Instantiate(dropOne, transform.position, Quaternion.identity);
            //AudioSource.PlayClipAtPoint(breakSound, transform.position);
            Destroy(gameObject);
        }
    }


}
