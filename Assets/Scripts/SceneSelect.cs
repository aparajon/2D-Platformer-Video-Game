using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelect : MonoBehaviour {

    [SerializeField] string levelToLoad;
    //[SerializeField] Vector3 nextStartingPosition;

    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void GotoFirstLevel()
	{
		SceneManager.LoadScene(1);
	}

    public void LoadLevelByName(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Level Load triger");
        if (collision.gameObject.tag == "Player")
        {
            LoadLevelByName(levelToLoad);
            //player.transform.position = nextStartingPosition;
            //player.GetComponent<PlayerControl>().canReceiveInput = true;
        }
    }
}
