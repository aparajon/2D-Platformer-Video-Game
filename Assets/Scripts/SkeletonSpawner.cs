using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSpawner : EnemySpawner {

    [SerializeField] bool guardMode = false;
    [SerializeField] bool usesAlertEffects = false;



    protected override void SpawnEnemy()
    {
        var alertController = Enemy.GetComponent<AlertGuardBehavior>();
        alertController.guardMode = this.guardMode;
        alertController.useAlertEffect = usesAlertEffects;

        base.SpawnEnemy();
     


    }
}
