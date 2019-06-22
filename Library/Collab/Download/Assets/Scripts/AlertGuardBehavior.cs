using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AlertGuardBehavior : EnemyBehavior {

    [SerializeField] public bool guardMode = false;



    [SerializeField] protected AudioClip alertSound;
    [SerializeField] protected float alertTimer = 15f;
    [SerializeField] protected float alertImageDuration = 5f;
    [SerializeField] protected GameObject alertImage;
    [SerializeField] protected float alertImageOffset = 1.75f;
    [SerializeField] protected float alertMoveSpeed = 2f;


    [SerializeField] protected float attackPause = 1.5f;
    [SerializeField] protected float jumpSpeed = 5;

    [SerializeField] protected bool drawDebugEnemyBoundary = false;
    [SerializeField] protected float enemyBoundary = 1f;
    [SerializeField] public bool useAlertEffect = true;


    protected bool showAlertImage = false;
    public float lastAlertTime = 0.0f;
    public float lastAttackTime = 0.0f;
   

    public bool isAttacking = false;
    public bool isAlerted = false;

    protected GameObject alertLocal;
  


    protected override void Start()
    {
        base.Start();
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();

        if (guardMode)
        {
            animator.SetBool("Guard", true);
            animator.SetBool("Guard", false);
        }
        else
        {
            animator.SetBool("Guard", false);
            animator.SetBool("Walking", true);
        }

        

        var alertPosition = new Vector2(transform.position.x, transform.position.y + alertImageOffset);
        alertLocal = Instantiate(alertImage, alertPosition, Quaternion.identity);
        alertLocal.transform.parent = transform;
        alertLocal.transform.localScale = Vector3.zero;

       
    }

    protected override void MoveForward()
    {
        if (player)
        {
            if (player.isDying || player.isRespawning)
            {
                return;
            }
        }
     

        base.MoveForward();
        animator.SetBool("Attacking", false);
        isAttacking = false;
    }
 

    public void Alert(bool effects = false)
    {
        //Debug.Log("Alert called.");

        isAlerted = true;
        guardMode = false;
        animator.SetBool("Guard", false);
        animator.SetBool("Walking", true);
        animator.SetFloat("WalkSpeed", 2 * defaultWalkingAnimSpeed);

        currentMoveSpeed = alertMoveSpeed;

        lastAlertTime = Time.time;

        if (effects)
        {
            ShowAlert();
            Invoke("HideAlert", alertImageDuration);

            if (alertSound)
            {
                AudioSource.PlayClipAtPoint(alertSound, transform.position);
            }
        }
    }

    protected void ShowAlert()
    {
        showAlertImage = true;
        alertLocal.transform.localScale = new Vector3(1, 1, 1);
    }

    protected void HideAlert()
    {
        showAlertImage = false;
        alertLocal.transform.localScale = new Vector3(0, 0, 0);
    }

    protected void Unalert()
    {
        isAlerted = false;
        HideAlert();
        currentMoveSpeed = baseMoveSpeed;
        animator.SetFloat("WalkSpeed", defaultWalkingAnimSpeed);
    }

    protected virtual void CheckUnalert()
    {
        var currentTime = Time.time;
        // Check if enemy should be un-alerted by checking last attack and hurt time
        // A high value in either means player is out of range for extended time

        var attackTimeDiff = currentTime - lastAttackTime;
        var hurtTimeDiff = currentTime - lastHurtTime;
        if (attackTimeDiff >= alertTimer)
        {
            // Debug.Log("Last hurt time " + lastHurtTime + " hurt time diff: " + hurtTimeDiff);
            if (hurtTimeDiff == 0f || hurtTimeDiff >= alertTimer)
            {
                Debug.Log("Enemy out of range for extended time period.");
                Unalert();
            }
        }
    }



    protected void SetAlertImage()
    {
        var currentTime = Time.time;
        var alertTimeDiff = currentTime - lastAlertTime;

        if (isAlerted && showAlertImage && alertTimeDiff < alertImageDuration)
        {
            // Debug.Log("Alert image picture");
            var alertPosition = new Vector2(transform.position.x, transform.position.y + alertImageOffset);
            alertLocal.transform.position = alertPosition;
        }
    }


    protected void StopAttackAnimation()
    {
        animator.SetBool("Walking", true);
        animator.SetBool("Attacking", false);
    }

    protected override void Hurt(float damage = 0)
    {
        if (!isAlerted)
        {
            Alert(false);
        }

        lastHurtTime = Time.time;

        base.Hurt(damage);
    }

    protected override void Die()
    {
        animator.SetBool("Attacking", false);
        base.Die();
    }

    protected void Jump()
    {
        //rigidBody2D.velocity += new Vector2(0, jumpSpeed);
        rigidBody2D.AddForce(transform.up * 2f * jumpSpeed);
    }

    protected GameObject CloseToEnemyFront()
    {
        var center = transform.GetComponent<Renderer>().bounds.center;
        float xOffset = .5f * Mathf.Sign(transform.localScale.x);
        var startPosition = center + new Vector3(xOffset, 0);

        int enemyLayerIndex = LayerMask.NameToLayer("Enemies");
        int layerMask = (1 << enemyLayerIndex);
        var direction = Vector2.left;
        if (isFacingRight())
        {
            direction = Vector2.right;
        }
        var hit = Physics2D.Raycast(startPosition, direction, 1f, layerMask);
        
        if (hit)
        {
            if (drawDebugEnemyBoundary)
            {
                Debug.DrawLine(center, hit.point, Color.cyan, 2f, false);
            }

            //Debug.Log(hit.collider.tag);

            if (hit.collider != null)
            {
                
                //Debug.Log("Close to enemy");
                return hit.collider.gameObject;
            }
        }

        return null;



    }



}
