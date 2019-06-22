using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using System;

public class MainMenu : MonoBehaviour {

	[SerializeField] Image heartIcon;
	[SerializeField] AudioClip cursorMove;
	[SerializeField] AudioClip selectedSound;

	enum Choice {NewGame, ExitGame}; // choices available to player.

	private Choice menuChoice;
	private Transform newLoc;
	private Transform exitLoc;
	private GameObject userInterface;
	private GameObject player;
	[SerializeField] GameObject UIPrefab;
	[SerializeField] GameObject XinPrefab;
	private bool choiceNotYetMade = true;

	// Use this for initialization
	void Start () {
		newLoc = transform.Find("NGIP");
		exitLoc = transform.Find("EIP");
		menuChoice = Choice.NewGame;
		SpawnIcon();
	}



	// Update is called once per frame
	void Update () {
		if (choiceNotYetMade) {
			MoveIcon();
			StartCoroutine(MenuChoice());
		}

	}

	// Places heart cursor on starting position (new game)
	void SpawnIcon()
	{
		heartIcon.transform.position = newLoc.position;
	}

	// Moves the heart cursor to player's choice position.
	void MoveIcon()
	{
		float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
		if (controlThrow == 0) { // no input on frame
			return;
		}
		else if (controlThrow > 0) { // player pressed 'up'
			if (menuChoice == Choice.ExitGame) {
				menuChoice = Choice.NewGame;
				heartIcon.transform.position = newLoc.position;
				AudioSource.PlayClipAtPoint(cursorMove, Camera.main.transform.position, 0.1f);
			}
			else { return; }
		}
		else { // player pressed 'down'
			if (menuChoice == Choice.NewGame) {
				menuChoice = Choice.ExitGame;
				heartIcon.transform.position = exitLoc.position;
				AudioSource.PlayClipAtPoint(cursorMove, Camera.main.transform.position, 0.1f);
			}
			else { return; }
		}
	}

	IEnumerator MenuChoice()
	{
		// If an action key is pressed
		if (CrossPlatformInputManager.GetButtonDown("Fire1") || CrossPlatformInputManager.GetButtonDown("Jump") || CrossPlatformInputManager.GetButtonDown("Submit")) {
			choiceNotYetMade = false;
			AudioSource.PlayClipAtPoint(selectedSound, Camera.main.transform.position, 0.1f);
			if (menuChoice == Choice.NewGame) { // fade screen then load scene after 3 seconds.
				Camera.main.SendMessage("FadeOut");
				yield return new WaitForSeconds(3f);
				SceneManager.LoadScene(1);
			} else if (menuChoice == Choice.ExitGame) { // quit game
				Application.Quit();
			}
		}
	}
}