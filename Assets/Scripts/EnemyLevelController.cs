using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLevelController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    

    public void ResetEnemies()
    {
        foreach (Transform child in transform)
        {
            var enemySpawner = child.GetComponent<EnemySpawner>();
            if (enemySpawner)
            {
                enemySpawner.Reset();
            }
        }
    } 
}
