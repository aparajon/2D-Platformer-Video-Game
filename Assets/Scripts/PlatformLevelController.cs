using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLevelController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ResetPlatforms()
    {
        foreach (Transform child in transform)
        {
            var platform = child.GetComponent<FloatingPlatform>();
            if (platform)
            {
                platform.Reset();
            }
        }
    }
}
