using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatform : MonoBehaviour
{
    [SerializeField] Vector3 velocity = new Vector3(1, 0, 0);
    [SerializeField] float leftRange = 4f;
    [SerializeField] float rightRange = 1f;
    [SerializeField] float downRange = 0f;
    [SerializeField] float upRange = 0f;
    [SerializeField] bool startRight = false;
    [SerializeField] bool startDown = false;
    [SerializeField] bool horizontalMode = true;
    [SerializeField] bool moveOnTouch = false;

    private bool moveActivated = false;

    private float leftBoundX;
    private float rightBoundX;

    private float lowerBoundY;
    private float upperBoundY;

    private bool movingRight = false;
    private bool movingDown = false;

    protected Rigidbody2D rigidBody2D;
    private GameObject playerChar;
    private Vector3 playerLocalScale;
    private BoxCollider2D collider;

    private Vector3 initPosition;

   

    void Start()
    {
        initPosition = transform.position;
        rigidBody2D = GetComponent<Rigidbody2D>();
        playerChar = GameObject.FindGameObjectWithTag("Player");
        playerLocalScale = playerChar.transform.localScale;
        collider = GetComponent<BoxCollider2D>();
        initializePlatform();
    }

    void initializePlatform()
    {
        transform.position = initPosition;
        var initPositionX = initPosition.x;
        var initPositionY = initPosition.y;

        leftBoundX = initPositionX - leftRange;
        rightBoundX = initPositionX + rightRange;

        lowerBoundY = initPositionY - downRange;
        upperBoundY = initPositionY + upRange;

        if (startRight)
        {
            movingRight = true;
        }

        if (startDown)
        {
            movingDown = true;
        }

        if (!moveOnTouch)
        {
            moveActivated = true;
        }
        else
        {
            moveActivated = false;
        }
    }

    // Update is called once per frame
    void Update()
    {



    }

    private void FixedUpdate()
    {
        if (moveOnTouch && !moveActivated)
        {
            return;
        }


        if (horizontalMode)
        {
            MoveX();
        }
        else
        {
            MoveY();
        }

    }

    private void MoveX()
    {
        var currentPositionX = transform.position.x;


        if (movingRight && currentPositionX < rightBoundX)
        {
            MoveRight();
        }

        else if (movingRight && currentPositionX >= rightBoundX)
        {
            MoveLeft();
        }

        else if (!movingRight && currentPositionX > leftBoundX)
        {
            MoveLeft();
        }

        else if (!movingRight && currentPositionX <= leftBoundX)
        {
            MoveRight();
        }

    }

    private void MoveY()
    {
        var currentPositionY = transform.position.y;


        if (movingDown && currentPositionY > lowerBoundY)
        {
            MoveDown();
        }

        else if (movingDown && currentPositionY <= lowerBoundY)
        {
            MoveUp();
        }

        else if (!movingDown && currentPositionY < upperBoundY)
        {
            MoveUp();
        }

        else if (!movingDown && currentPositionY >= upperBoundY)
        {
            MoveDown();
        }

    }


    private void MoveRight()
    {
        transform.position += velocity * Time.deltaTime;
        movingRight = true;

    }

    private void MoveLeft()
    {
        transform.position -= velocity * Time.deltaTime;
        movingRight = false;
    }

    private void MoveDown()
    {
        transform.position -= velocity * Time.deltaTime;
        movingDown = true;
    }

    private void MoveUp()
    {
        transform.position += velocity * Time.deltaTime;
        movingDown = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Platform: On collision enter");
        if (collision.gameObject.tag == "Player" && collision.collider.GetType() == typeof(BoxCollider2D))
        {
            collision.collider.transform.SetParent(transform);
            moveActivated = true;
            //collision.collider.transform.localScale = playerLocalScale;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Platform: On collision exit");

        if (collision.gameObject.tag == "Player" && collision.collider.GetType() == typeof(BoxCollider2D))
        {
            collision.collider.transform.SetParent(null);
			DontDestroyOnLoad(playerChar);
			//collision.collider.transform.localScale = playerLocalScale;
		}
    }

    public void Reset()
    {
        initializePlatform();
    }
}
