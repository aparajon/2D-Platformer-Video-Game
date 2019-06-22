using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class DialogTrigger : MonoBehaviour {

	enum TriggerType {Character, Story, Event, Chest};

	public bool canTalk;

	private Collider2D thisCollider;
	private Collider2D playerCollider;
	private GameObject playerChar;

	public bool onlyPlayOnce;

	[SerializeField] Dialog dialog;
	[SerializeField] TriggerType triggerType;
	[SerializeField] bool playDialog;



	private void Start()
	{
		// If the Story/Chest/Event types are chosen, it can only be played once so set automatically.
		if (triggerType == TriggerType.Story || triggerType == TriggerType.Chest || triggerType == TriggerType.Event) { onlyPlayOnce = true; }
		thisCollider = GetComponent<Collider2D>();
		playerChar = GameObject.FindGameObjectWithTag("Player");
		playerCollider = playerChar.GetComponent<CapsuleCollider2D>();

		canTalk = true; // set to false when dialog begins, making player unable to start another dialog.
	}

	private void Update()
	{
		if (canTalk == true) {
			TriggerDialog();
		} else {
			NextLine();
		}
	}

	public void TriggerDialog()
	{
		if (playDialog == true) {
			// If Story, then auto play only once.
			if (triggerType == TriggerType.Story && onlyPlayOnce == true) {
				FindObjectOfType<DialogManager>().StartDialog(dialog);
				StopPlayerMoveOnDialog();
				onlyPlayOnce = false;
			}
			// If chest, then queue chest animation and only place once.
			else if (triggerType == TriggerType.Chest && onlyPlayOnce == true) {
				if (thisCollider.IsTouching(playerCollider) && CrossPlatformInputManager.GetButtonDown("Fire1")) {
					TreasureChest chest = transform.parent.GetComponent<TreasureChest>();
					FindObjectOfType<DialogManager>().StartDialog(dialog);
					StopPlayerMoveOnDialog();
					onlyPlayOnce = false;
					chest.IsOpened();
				}
			}
			// If character, then allow talk multiple times.
			else if (triggerType == TriggerType.Character) {

				if (thisCollider.IsTouching(playerCollider) && CrossPlatformInputManager.GetButtonDown("Fire1")) {
					FindObjectOfType<DialogManager>().StartDialog(dialog);
					StopPlayerMoveOnDialog();
				}
			}
			else {
				return;
			}
		}

	}

	public void TriggerEvent()
	{
		if (triggerType == TriggerType.Event && canTalk == true && onlyPlayOnce == true) {
			FindObjectOfType<DialogManager>().StartDialog(dialog);
			StopPlayerMoveOnDialog();
			onlyPlayOnce = false;
		}
	}

	private void StopPlayerMoveOnDialog()
	{
		canTalk = false;
		playerChar.GetComponent<PlayerControl>().canMove = 0;
	}

	public void SetCanTalk(bool type)
	{
		canTalk = type;
	}

	public void NextLine()
	{
		if (CrossPlatformInputManager.GetButtonDown("Fire1")) {
			FindObjectOfType<DialogManager>().DisplayNextSentence();
		}
	}
}