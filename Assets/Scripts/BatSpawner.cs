using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatSpawner : EnemySpawner {

    [SerializeField] float leftFlightRange;
    [SerializeField] float rightFlightRange;
    [SerializeField] float downFlightRange;

    protected override void SpawnEnemy()
    {
        var batController = Enemy.GetComponent<BatBehavior>();
        batController.leftFlightRange = this.leftFlightRange;
        batController.rightFlightRange = this.rightFlightRange;
        batController.downFlightRange = this.downFlightRange;

        base.SpawnEnemy();

    }
}
