using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGameScript : MonoBehaviour {

	GameObject userInterface;
	DialogManager dialogManager;
	DialogTrigger storyTrigger;

	GameObject player;
	AsyncOperation asyncLoad;
	[SerializeField] GameObject Xin;
	[SerializeField] Vector3 playerStartPosition;
	[SerializeField] Vector3 playerSpawnPosition;
	bool notYetStarted;

	// Use this for initialization
	void Start () {
		InstantiatePlayer();
		userInterface = GameObject.Find("User Interface");
		dialogManager = userInterface.GetComponentInChildren<DialogManager>();

		storyTrigger = GameObject.Find("WinScript").GetComponent<DialogTrigger>();

		//GameObject.Find("WinScript").GetComponent<DialogTrigger>().TriggerEvent();
		notYetStarted = true;
	}

	
	// Update is called once per frame
	void Update () {
		if (notYetStarted == true) {
			notYetStarted = false;
			StartCoroutine(CheckCreditsDone());
		}

	}

	private IEnumerator CheckCreditsDone()
	{
		yield return new WaitUntil(() => dialogManager.GetStillPlaying() == false);

		AsyncOperation async = SceneManager.LoadSceneAsync(0);

		while (!async.isDone) {
			yield return async;
		}
	}

	private void InstantiatePlayer()
	{
		player = GameObject.FindGameObjectWithTag("Player");

		if (!player) {
			// player
			player = Instantiate(Xin, playerStartPosition, Quaternion.identity);
			player.name = "Xin";
			player.GetComponent<HealthIndicator>().SetNumberHearts(3);
			player.GetComponent<HealthIndicator>().SetHealth(6);

		}
		else {
			player.transform.position = playerStartPosition;
		}
		player.GetComponent<PlayerControl>().spawnPosition = playerSpawnPosition;
	}

}
