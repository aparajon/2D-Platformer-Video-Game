using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewerDialogueController : MonoBehaviour {
    SewerLevel park;
    protected bool gotKey;
    // Use this for initialization
    void Start () {
        park = GameObject.FindGameObjectWithTag("Level").GetComponent<SewerLevel>();
        gotKey = false;
    }

    // Update is called once per frame
    void Update () {
        checkForKeys();	
	}

    private void checkForKeys()
    {
        if (park.lunaKey == 1 && gotKey == false)
        {
            gotKey = true;
            changeDialogue();
        }
    }

    private void changeDialogue()
    {
        transform.Translate(-2.5f, 0.0f, 0.0f); //moves the dialogue box down one unit
    //destroys the parent option once you have the key\
    }

}
