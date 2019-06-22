using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour {

	public static DialogManager instance = null;

	private GameObject playerChar;
	private Text charNameBox;
	private Text charDialogBox;
	private Image charImageBox;
	private Animator animator;
	private Queue<string> sentences;
	private Queue<string> names;
	private Queue<Sprite> images;

	[SerializeField] AudioClip talkSound;

	private bool stillPlaying;

	// Implement singleton behavior.
	private void Awake()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		playerChar = GameObject.FindGameObjectWithTag("Player");
		animator = GetComponent<Animator>();
		charNameBox = transform.Find("NameArea").GetComponent<Text>();
		charDialogBox = transform.Find("DialogArea").GetComponent<Text>();
		charImageBox = transform.Find("CharPortrait").GetComponent<Image>();
		if (!charNameBox) {
			Debug.Log("Couldnt find");
		}
		charImageBox.color = Color.clear;

		sentences = new Queue<string>();
		names = new Queue<string>();
		images = new Queue<Sprite>();

		stillPlaying = true;

	}

	public void StartDialog(Dialog dialog)
	{
		stillPlaying = true;

		charNameBox.text = "";
		charDialogBox.text = "";
		charImageBox.color = Color.clear;
		charImageBox.sprite = null;

		// Set animator to drop down dialog box.
		animator.SetBool("isOpenDialog", true);

		// Clear any previous names and sentences in the queues.
		names.Clear();
		sentences.Clear();

		// Place all names and sentences from inspector into queues.
		foreach(string s in dialog.charDialog) {
			sentences.Enqueue(s);
		}
		foreach (string n in dialog.charName) {
			names.Enqueue(n);
		}
		foreach (Sprite i in dialog.charImage) {
			images.Enqueue(i);
		}

		// Write first sentence.
		Invoke("DisplayNextSentence", 0.5f);
	}


	public void DisplayNextSentence()
	{	
		// If finished, call EndDialog().
		if (sentences.Count == 0) {
			EndDialog();
			return;
		}
		else {
			// Otherwise dequeue top name and sentence and type sentence.
			string sentence = sentences.Dequeue();
			string name = names.Dequeue();
			Sprite image = images.Dequeue();
			StopAllCoroutines();
			StartCoroutine(TypeSentence(sentence, name, image));
		}
	}

	IEnumerator TypeSentence(string sentence, string name, Sprite image)
	{

		// Turn off player running animation.
		if (playerChar.GetComponent<Animator>().GetBool("isRunning")) {
			playerChar.GetComponent<Animator>().SetBool("isRunning", false);
			playerChar.GetComponent<PlayerControl>().turnOffFootsteps();
		}

		// Set image to image box.
		charNameBox.text = name;
		charImageBox.preserveAspect = false;
		charImageBox.sprite = image;
		charImageBox.color = Color.white;

		// Write name and sentence (letter by letter) to screen.
		charDialogBox.text = "";
		int skipLetter = 0;
		foreach (char letter in sentence.ToCharArray()) {
			skipLetter++;
			if (skipLetter % 3 == 1) {
				AudioSource.PlayClipAtPoint(talkSound, GameObject.FindGameObjectWithTag("Player").transform.position, 0.25f);
			}
			charDialogBox.text += letter;
			yield return null;
		}
	}

	private void EndDialog()
	{
		animator.SetBool("isOpenDialog", false);
		// Set canTalk member in all DialogTrigger objects to true. 
		DialogTrigger[] allTriggers = FindObjectsOfType<DialogTrigger>();
		foreach (DialogTrigger t in allTriggers) {
			t.SendMessage("SetCanTalk", true);
		}
		// Let player move again (this was set to 0 in DIalogTrigger.cs) and reset textboxes.
		playerChar.GetComponent<PlayerControl>().CanMove(1);

		stillPlaying = false;
	}

	public bool GetStillPlaying()
	{
		return stillPlaying;
	}

	public void SetStillPlaying(bool option)
	{
		stillPlaying = option;
	}
}
