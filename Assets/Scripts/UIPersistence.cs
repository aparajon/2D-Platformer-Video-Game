using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

public class UIPersistence : MonoBehaviour {

	int numDeaths = 0;

	// Implementing singleton behavior.
	static bool wasCreated = false;
	
	private void Awake()
	{
		if (!wasCreated) {
			DontDestroyOnLoad(this.gameObject);
			wasCreated = true;
		}
		else {
			Destroy(this.gameObject);
		}
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		EndGame();
		CountDeaths();
	}

	void EndGame()
	{
		if (CrossPlatformInputManager.GetButton("Cancel")) {
			Application.Quit();
		}
	}

	public void CountDeaths()
	{
		numDeaths++;
	}
}
