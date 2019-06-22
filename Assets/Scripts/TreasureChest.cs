using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour {

	Animator animator;
	GameObject level;
	[SerializeField] int goldAmount;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		level = GameObject.FindGameObjectWithTag("Level");
	}
	
	public void IsOpened()
	{
		animator.SetBool("isOpenChest", true);
		ParkLevel levelScript = level.GetComponent<ParkLevel>();
		levelScript.AddLunaKey();
    }

    public void IsClosed()
	{
		animator.SetBool("isOpenChest", false);
	}
}
