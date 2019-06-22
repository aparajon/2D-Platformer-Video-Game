using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class BossDoor : MonoBehaviour {

    [SerializeField] AudioClip collapseSound;
    [SerializeField] AudioClip fallSound;


    GameObject playerChar;
    CapsuleCollider2D playerCollider;
    BoxCollider2D collider;
    GameObject foreground;
 
    List<Vector3> bridgeTiles;
    ParkLevel park;
    AudioSource bgMusic;

    bool playerOpenedDoorWithAllKeys = false;
    bool bridgeIsCollapsing = false;

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        playerChar = GameObject.FindGameObjectWithTag("Player");
        playerCollider = playerChar.GetComponent<CapsuleCollider2D>();
        foreground = GameObject.FindGameObjectWithTag("Foreground");
    }

    // Update is called once per frame
    void Update()
    {
        if (collider.IsTouching(playerCollider) && CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            StartCoroutine(LoadLunaLevel());
        }

 

    }

    private IEnumerator LoadLunaLevel()
    {
        yield return new WaitForSeconds(.5f);

        SceneSelect s = new SceneSelect();
        s.LoadLevelByName("4.BossFight");
    }

  
}
