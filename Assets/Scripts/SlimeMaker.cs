using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMaker : MonoBehaviour {

    [SerializeField] GameObject drop;
    private float STimer;
    private IEnumerator slimeTime;

    // Use this for initialization
    void Start () {
        StartCoroutine(CreateSlime());
    }

    // Update is called once per frame
    void Update () {
	}

    IEnumerator CreateSlime()
    {
        while (true)
        {
            STimer = Random.Range(.5f, 2.5f);
            yield return new WaitForSeconds(STimer);
            Instantiate(drop, transform.position, Quaternion.identity);
        }
    }
}
