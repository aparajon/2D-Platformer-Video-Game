using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class EndSewerLevel : MonoBehaviour {

    [SerializeField] string levelToLoad;
    private GameObject player;
    public int levelTransitions;
    BoxCollider2D collider;
    CapsuleCollider2D playerCollider;


    // Use this for initialization
    void Start () {
        levelTransitions = 0;
        player = GameObject.FindGameObjectWithTag("Player");
        collider = GetComponent<BoxCollider2D>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update () {
        checkForShoot();
        transitionLevel();
    }
    public void LoadLevelByName(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    private void checkForShoot()
    {
        if (collider.IsTouching(playerCollider))
        {
            if (CrossPlatformInputManager.GetButtonDown("Fire1"))
            {
                levelTransitions = levelTransitions + 1;
            }
        }
    }

    private void transitionLevel()
    {
        if (levelTransitions == 3)
        {
            LoadLevelByName(levelToLoad);
        }
    }


}
