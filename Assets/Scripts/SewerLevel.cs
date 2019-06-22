using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SewerLevel : MonoBehaviour {


    GameObject player;
    [SerializeField] public int lunaKey;
    [SerializeField] GameObject Xin;
    [SerializeField] Vector3 playerStartPosition;
    [SerializeField] Vector3 playerSpawnPosition;

    private void Start()
    {
        lunaKey = 0;
        GameObject.FindGameObjectWithTag("LunaKey").GetComponentInChildren<Text>().text = "0";


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
    }

    private void Update()
    {
        GameObject.FindGameObjectWithTag("LunaKey").GetComponentInChildren<Text>().text = lunaKey.ToString();
    }

    IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    public void AddLunaKey()
    {
        lunaKey++;
    }

}
