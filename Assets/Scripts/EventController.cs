using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventController : MonoBehaviour {

    public bool eventStarted = false;
    public bool startEvent = false;

    // Use this for initialization
    protected virtual void Start () {
        Debug.Log("Event controller start");
        startEvent = false;

    }
	
	// Update is called once per frame
	protected virtual void Update () {
        if (!eventStarted && startEvent)
        {
            StartCoroutine(EventRoutine());
        }
	}

    public void StartEvent()
    {
        Debug.Log("Event controller: event started");
        startEvent = true;
    }

    protected virtual IEnumerator EventRoutine()
    {
        eventStarted = true;
        Debug.Log("Event controller: EventRoutine()");

        yield return true; 
    }
}
