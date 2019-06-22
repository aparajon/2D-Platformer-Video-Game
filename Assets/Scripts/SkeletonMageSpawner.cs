using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMageSpawner : SkeletonSpawner {

    public List<GameObject> minions;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    
    }

    protected override void SpawnEnemy()
    {
        var controller = Enemy.GetComponent<SkeletonMageBehavior>();
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

            minions = newEnemy.GetComponent<SkeletonMageBehavior>().minionList;
        }
    }


    public override void Reset()
    {
        
        foreach (var minion in minions)
        {
            Destroy(minion);
        }
        minions.Clear();

        base.Reset();
    }

}
