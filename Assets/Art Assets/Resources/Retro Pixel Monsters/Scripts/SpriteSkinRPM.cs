using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class SpriteSkinRPM : MonoBehaviour {
	[Tooltip("The new spritesheet texture to use. Set as the same as source texture to switch outline styles.")]
	public Texture2D newSprite;
	[Tooltip("The path to the new spritesheet. Note that the sheets must be in the Resources folder.")]
	public string folderPath = "Retro Pixel Monsters/Spritesheets/";
	[Tooltip("Toggle using the color outline variation of a sprite. Offsets frame index.")]
	public bool useColorOutline = true;

	private Sprite[] newSpritesheet; //Spritesheet array. Populated with all sprites within a texture sheet.
	private SpriteRenderer parentRenderer; //The object's sprite renderer.
	private string currentFrameName; //Name of the current frame. Used to find the current frame's number.
	private int frameIndex = 0; //The index of the current frame. Used to set index of new frame.

	//During start, find the SpriteRenderer and load all spritesheet frames into the array.
	//Again, spritesheets need to be in the Resources folder for the Resources.LoadAll to work.
	void Start () {
		if (parentRenderer == null) {
			parentRenderer = GetComponentInParent<SpriteRenderer>();
		}
		if (newSprite != null) {
			newSpritesheet = Resources.LoadAll<Sprite>(folderPath + newSprite.name);
		}
	}

	//Using LateUpdate, since trying to replace sprites in Update will just be overridden by the animation.
	void LateUpdate () {
		//If we've got our spritesheet, our renderer and the spritesheet is actually a sheet, let's fire up the magic!
		if (newSprite != null && parentRenderer != null && newSpritesheet.Length > 0){
			//Get the currently rendered sprite's name.
			currentFrameName = parentRenderer.sprite.name;
			//Parse out the frame number from the frame name and use as index for the new frame to render.
			int.TryParse(Regex.Replace(currentFrameName, "[^0-9]", ""), out frameIndex);
			//If we're NOT using the color outline variations, offset the sprite frame index.
			if (useColorOutline == false) {
				frameIndex += newSpritesheet.Length / 2;
			}
			//Finally, set the new sprite to render.
			parentRenderer.sprite = newSpritesheet[frameIndex];
		}
		else if (newSprite == null){
			//If the New Sprite has not been set, log a warning and then disable this script (to prevent a new warning every frame.)
			Debug.LogWarning("New Sprite has not been set. Drag and drop your spritesheet texture to the New Sprite field.");
			this.enabled = false;
		}
		else if (parentRenderer == null) {
			//If the object contains no Sprite Renderer, log an error and disable script.
			Debug.LogError("Parent does not contain a Sprite Renderer.");
			this.enabled = false;
		}
		else if (newSpritesheet.Length <= 0) {
			//If the new sprite sheet fails loading any sprites, it might not be a proper spritesheet. Could also be that the sprite wasn't found in the folder path.
			Debug.LogWarning("It seems there were too few sprites in the New Sprite. Was it all a ruse?! Actually, it might be the wrong Folder Path, too.");
			this.enabled = false;
		}
	}
}
