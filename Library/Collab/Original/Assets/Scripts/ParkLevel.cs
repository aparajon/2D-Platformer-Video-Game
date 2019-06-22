using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ParkLevel : MonoBehaviour {

	[SerializeField] int lunaKey;
	GameObject player;

	private void Start()
	{
		lunaKey = 0;
		player = GameObject.FindGameObjectWithTag("Player");
		//player.transform.position = new Vector2(-26f, -3.8f);
		player.GetComponent<HealthIndicator>().SetNumberHearts(3);
		player.GetComponent<HealthIndicator>().SetHealth(6);
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
		if (lunaKey >= 4) {
			yield return new WaitUntil(() => dialogManager.GetStillPlaying() == false);
			storyTrigger.TriggerEvent();
			yield return new WaitUntil(() => dialogManager.GetStillPlaying() == false);
			SceneManager.LoadScene(0);
		}
	}

	IEnumerator Wait(float seconds)
	{
		yield return new WaitForSeconds(seconds);
	}


}
