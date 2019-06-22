using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ParkLevel : MonoBehaviour {

	[SerializeField] public int lunaKey;
    [SerializeField] GameObject Xin;
    [SerializeField] Vector3 playerStartPosition;
    [SerializeField] Vector3 playerSpawnPosition;
    GameObject player;
  

    private void Start()
	{
        //lunaKey = 4;
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
    }

	private void Update()
	{
		GameObject.FindGameObjectWithTag("LunaKey").GetComponentInChildren<Text>().text = lunaKey.ToString();
		StartCoroutine(CheckAllKeysFound());
		
	}

	public void AddLunaKey()
	{
		lunaKey++;
	}

	IEnumerator CheckAllKeysFound()
	{
		DialogTrigger storyTrigger = GetComponent<DialogTrigger>();
		DialogManager dialogManager = FindObjectOfType<DialogManager>();
		yield return null;
		/*if (lunaKey >= 4) {
			yield return new WaitUntil(() => dialogManager.GetStillPlaying() == false);
			storyTrigger.TriggerEvent();
			yield return new WaitUntil(() => dialogManager.GetStillPlaying() == false);
			SceneManager.LoadScene(0);
		}*/
	}

	IEnumerator Wait(float seconds)
	{
		yield return new WaitForSeconds(seconds);
	}


}
