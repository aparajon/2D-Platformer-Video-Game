using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchManager : MonoBehaviour {

    private int numChildren;
    private bool firstMessage;
    private bool secondMessage;
    private bool thirdMessage;

    // Use this for initialization
    void Start () {
        numChildren = 3;
        firstMessage = true;
        secondMessage = true;
        thirdMessage = true;

    }
	
	// Update is called once per frame
	void Update () {
        checkChildren();
	}

    private void checkChildren()
    {
        numChildren = transform.childCount;
        if (numChildren == 2 && firstMessage)
        {
            GameObject.Find("DestroySwitchOne").GetComponent<DialogTrigger>().TriggerEvent();
            firstMessage = false;

        }
        if (numChildren == 1 && secondMessage)
        {
            GameObject.Find("DestroySwitchTwo").GetComponent<DialogTrigger>().TriggerEvent();
            secondMessage = false;
        }
        if (numChildren == 0 && thirdMessage)
        {
            GameObject.Find("DestroySwitchThree").GetComponent<DialogTrigger>().TriggerEvent();
            thirdMessage = false;
        }
    }

}

