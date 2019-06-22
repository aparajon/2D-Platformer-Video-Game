using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBar : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        OnDestroy();
    }
//destroys self if less than two children
    private void OnDestroy()
    {
        if (transform.childCount != 2) // if this object is the last child
            {
                Destroy(gameObject, 0.1f); // destroy parent a few frames later
            }
        }
 }
