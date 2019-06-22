using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFade : MonoBehaviour {

	public Texture2D fadeText;
	public float fadeSpeed = 0.2f;
	public bool fadeToggle = true;
	
	private float alpha = 1f;
	private float fadeDir = -1f;

	// Use this for initialization
	void Start () {
		FadeIn();
	}

	private void OnGUI()
	{
		if (fadeToggle) {
			alpha += fadeDir * fadeSpeed * Time.deltaTime;
			alpha = Mathf.Clamp01(alpha);

			Color newColor = GUI.color;
			newColor.a = alpha;
			GUI.color = newColor;
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeText);
		} else {
			Debug.Log("Fade toggler is turned off.");
			return;
		}
	}

	void FadeIn()
	{
		fadeDir = -1f;
	}

	void FadeOut()
	{
		fadeDir = 1f;
	}


}
