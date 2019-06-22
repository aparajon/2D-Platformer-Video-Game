using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewerKey : MonoBehaviour {

    GameObject level;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        level = GameObject.FindGameObjectWithTag("Level");
        if (collision.gameObject.tag == "Player")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            SewerLevel levelScript = level.GetComponent<SewerLevel>();
            levelScript.AddLunaKey();
            Destroy(gameObject);
        }
    }


}
