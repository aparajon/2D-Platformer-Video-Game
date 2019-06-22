using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LairLevel : MonoBehaviour {

    [SerializeField] GameObject Xin;
    [SerializeField] Vector3 playerStartPosition;
    [SerializeField] Vector3 playerSpawnPosition;
    GameObject player;
    //Checkpoint checkpoint;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (!player)
        {
            // player
            player = Instantiate(Xin, playerStartPosition, Quaternion.identity);
            player.name = "Xin";
            player.GetComponent<HealthIndicator>().SetNumberHearts(3);
            player.GetComponent<HealthIndicator>().SetHealth(6);

        }
        else
        {
            player.transform.position = playerStartPosition;
        }

        player.GetComponent<PlayerControl>().spawnPosition = playerSpawnPosition;
        player.GetComponent<PlayerControl>().canReceiveInput = true;
        //checkpoint = GameObject.FindGameObjectWithTag("Checkpoint").GetComponent<Checkpoint>();

    }

    private void Update()
    {

    }

    IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
