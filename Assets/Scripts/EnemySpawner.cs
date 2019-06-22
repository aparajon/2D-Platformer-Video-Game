using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField] protected GameObject Enemy;
    [SerializeField] protected float spawnRate = 5f;
    [SerializeField] protected int baseSpawnCount = 1;
    [SerializeField] protected bool singleSpawnMode = true;
    [SerializeField] protected float singleSpawnDelay = 5f;
    [SerializeField] protected bool startsFacingRight = false;
    [SerializeField] protected bool useDropEvent = false;
    [SerializeField] protected GameObject dropObject;
    [SerializeField] protected Vector3 dropObjectPosition;

    protected List<GameObject> enemies = new List<GameObject>();
    protected float lastSpawnTime = 0.0f;
    protected bool singleSpawnActive = false;
    protected int currentSpawnCount = 0;

    private float gizmoWidth = 10.0f;
    private float gizmoHeight = 5.0f;

    protected float lastPosition;
    private bool eventStarted = false;
    private bool lockEvent = false;

    private GameObject drop;

    // Use this for initialization
    protected virtual void Start () {
        currentSpawnCount = baseSpawnCount;
        SpawnEnemy();
    }

    // Update is called once per frame
    protected virtual void Update() {
        //Check enemyList
        CheckEnemies();


        if (currentSpawnCount > 0)
        {
            if (singleSpawnMode)
            {
                if (enemies.Count == 0 && !singleSpawnActive)
                {
                    singleSpawnActive = true;
                    Invoke("SpawnEnemy", singleSpawnDelay);
                }
            }
        }
        else
        {
            if (enemies.Count == 0 && !singleSpawnActive)
            {
                if (useDropEvent && !lockEvent && !eventStarted)
                {
                    StartFinalEvent();
                }

            }

        }

      
	}

    protected void CheckEnemies()
    {
        foreach (var e in enemies)
        {
            if (e == null)
            {
                enemies.Remove(e);
            }
        }
    }

    protected virtual void SpawnEnemy()
    {
        var controller = Enemy.GetComponent<EnemyBehavior>();
        if (controller)
        {
            controller.startsRight = startsFacingRight;
        }
       

        if (Enemy)
        {
            var newEnemy = Instantiate(Enemy, transform.position, Quaternion.identity);
            enemies.Add(newEnemy);
            var localScaleX = transform.localScale.x;
            var localScaleY = transform.localScale.y;

            currentSpawnCount--;
            singleSpawnActive = false;

        }
    }

    public virtual void Reset()
    {
        lockEvent = true;
        foreach (var e in enemies)
        {
            Destroy(e);
        }

        if (drop)
        {
            Destroy(drop);
        }

        enemies.Clear();
        currentSpawnCount = baseSpawnCount;
        SpawnEnemy();
        lockEvent = false;
        eventStarted = false;
    }

    public void StartFinalEvent()
    {
        Debug.Log("Starting drop event");
        eventStarted = true;
        //if (finalEvent)
        //{
        //    var eventController = finalEvent.GetComponent<EventController>();
        //    eventController.StartEvent();
        //}

        //For now just instantiate object
        drop = Instantiate(dropObject, dropObjectPosition, Quaternion.identity);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gizmoWidth, gizmoHeight));
    }

 
}
