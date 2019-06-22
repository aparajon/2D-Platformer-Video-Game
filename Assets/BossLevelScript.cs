using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossLevelScript : MonoBehaviour {

	GameObject player;
	GameObject luna;

	GameObject userInterface;
	DialogManager dialogManager;
	DialogTrigger storyTrigger;

	[SerializeField] GameObject lunaHeart;
	[SerializeField] AudioClip lunaHeartBeep;
	[SerializeField] GameObject lux;
	AudioSource audioSource;
	public int startBattleToggle;

	[SerializeField] GameObject Xin;
	[SerializeField] Vector3 playerStartPosition;
	[SerializeField] Vector3 playerSpawnPosition;

	private void Awake()
	{
		InstantiatePlayer();
	}

	// Use this for initialization
	void Start () {


		luna = GameObject.Find("Luna");
		userInterface = GameObject.Find("User Interface");
		dialogManager = userInterface.GetComponentInChildren<DialogManager>();
		audioSource = GameObject.Find("Music").GetComponentInChildren<AudioSource>();
		storyTrigger = luna.GetComponent<DialogTrigger>();

		SetPosition();
		audioSource.Stop();
		startBattleToggle = 0;
		luna.GetComponent<LunaBehavior>().attackSkipToggle = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (dialogManager.GetStillPlaying() == false && startBattleToggle == 0) {
			player.GetComponent<PlayerControl>().canMove = 0;
			startBattleToggle = 1;
			audioSource.Play();
			StartCoroutine(SetLunaUI());
		}
		else if (player.GetComponent<PlayerControl>().canMove > 0 && dialogManager.GetStillPlaying() == true) {
			player.GetComponent<PlayerControl>().canMove = 0;
		}

		StartCoroutine(CheckLunaDeath());
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

	private IEnumerator CheckLunaDeath()
	{
		if (luna.GetComponent<LunaBehavior>().lunaDied == true) {
			luna.GetComponent<LunaBehavior>().attackSkipToggle = false;
			player.GetComponent<PlayerControl>().canMove = 0;
			player.GetComponent<PlayerControl>().TurnInvulnerable(10f);
			DeleteLunaUI();
			audioSource.Stop();
			yield return new WaitUntil(() => dialogManager.GetStillPlaying() == false);
			GameObject.Find("WinScript").GetComponent<DialogTrigger>().TriggerEvent();
			yield return new WaitUntil(() => dialogManager.GetStillPlaying() == false);

			// Spawn Lux
			Instantiate(lux, new Vector2(-4.5f, -2.5f), Quaternion.identity);
			AudioSource.PlayClipAtPoint(lunaHeartBeep, player.transform.position);
			yield return new WaitForSeconds(0.1f);

			GameObject.Find("WinScript2").GetComponent<DialogTrigger>().TriggerEvent();
			yield return new WaitUntil(() => dialogManager.GetStillPlaying() == false);

			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}
	}

	private IEnumerator SetLunaUI()
	{
		float offset = 245f;

		for (int i = 0; i < 8; i++) {
			var LunaHeart = Instantiate(lunaHeart, new Vector2(0, 0), Quaternion.identity, userInterface.transform);
			LunaHeart.GetComponent<RectTransform>().anchoredPosition = new Vector2(offset, 168.3f);
			offset += 15;
			AudioSource.PlayClipAtPoint(lunaHeartBeep, player.transform.position);
			yield return new WaitForSeconds(0.2f);
		}
		StartBattle();
	}

	public void DeleteLunaUI()
	{
		Transform[] LunaHeart = new Transform[8];
		int countHearts = 0;

		foreach (Transform child in userInterface.transform) {
			if (child.name == "LunaHeart(Clone)") {
				LunaHeart[countHearts] = child.transform;
				countHearts++;
			}
		}

		foreach (Transform heart in LunaHeart) {
			Destroy(heart.gameObject);
		}

	}

	void SetPosition()
	{
		if (player != null) {
			player.transform.position = new Vector2(-12.6f, -4.76f);
		}
		else {
			player.transform.position = new Vector2(-12.6f, -4.76f);
		}
	}

	private void StartBattle()
	{
		luna.GetComponent<LunaBehavior>().attackSkipToggle = true;
		player.GetComponent<PlayerControl>().canMove = 1;
	}
}
