using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthIndicator : MonoBehaviour {

	// Set these Sprites to heart icons.
	public Sprite fullHeart;
	public Sprite halfHeart;
	public Sprite emptyHeart;

	// Toggle health and total number of collected hearts here.
	private int health = 6;
	private int numberOfHearts = 3; // should be private in the future.

	// Create a hearts array to manipulate the hearts in the User Interface GameObject.  
	// Set the total number of hearts available in game.  If this changes, User Interface hearts must be updated as well.
	static private int totalNumberOfHeartsInGame = 10;
	private Image[] hearts = new Image[totalNumberOfHeartsInGame];

	static bool wasCreated = false;
    //triggers animations

    private void Awake()
	{
		// Only one set of hearts should persist in game.
		if (!wasCreated) {
			DontDestroyOnLoad(this.gameObject);
			wasCreated = true;
		}
		else {
			Destroy(this.gameObject);
		}
	}

	// Use this for initialization.
	void Start () {
        // Find the User Interface GameObject and obtain all of its children Images.
        GameObject tempCanvas = GameObject.Find("User Interface");
		if (tempCanvas) {
			Image[] canvasImages = tempCanvas.GetComponentsInChildren<Image>();

			// Place all UI Heart images into hearts[].
			int heartsIterator = 0;
			for (int i = 0; i < canvasImages.Length; i++) {
				if (canvasImages[i].name == "Heart") {
					hearts[heartsIterator] = canvasImages[i];
					heartsIterator++;
				}
			}
		}
		else {
			Debug.Log("Could not find User Interface.  Please place User Interface prefab into scene.");
		}
	}
	
	// Update is called once per frame
	void Update () {
		UpdateHealth();
	}

	// Used to update the health of the character as they take damage or gain more health hearts.
	private void UpdateHealth()
	{
		// Check that the maximum number of gained hearts does not exceed game max.
		if (numberOfHearts > totalNumberOfHeartsInGame) { numberOfHearts = totalNumberOfHeartsInGame; }

		// Check to make sure health cannot go above number of total collected hearts.
		if (health > 2 * numberOfHearts) { health = 2 * numberOfHearts; }

		// Draw in a full, half, or empty heart depending on health of player.
		for (int i = 0; i < hearts.Length; i++) {
			if (i * 2 < health) {
				if (( i - health / 2 ) == 0) {
					hearts[i].sprite = halfHeart;
				}
				else {
					hearts[i].sprite = fullHeart;
				}
			} else {
				hearts[i].sprite = emptyHeart;
			}

			// Draw in only the number of hearts which he player has collected.
			if (i < numberOfHearts) {
				hearts[i].enabled = true;
			}
			else {
				hearts[i].enabled = false;
			}
		}
	}

	public int GetHealth()
	{
		return health;
	}

	public int GetNumberOfHearts()
	{
		return numberOfHearts;
	}

	public void SetHealth(int setTo)
	{
		health = setTo;
	}

	public void SetNumberHearts(int setTo)
	{
		numberOfHearts = setTo;
	}

	public void DecreaseHealth(int damage)
	{
		health -= damage;
	}

	public void IncreaseHealth(int amount)
	{
		health += amount;
	}

}
