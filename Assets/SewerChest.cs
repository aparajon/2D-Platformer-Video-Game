using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewerChest : MonoBehaviour {

    Animator animator;
    GameObject level;
    [SerializeField] int goldAmount;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        level = GameObject.FindGameObjectWithTag("Level");
    }

    public void IsOpened()
    {
        animator.SetBool("isOpenChest", true);
        SewerLevel levelScript = level.GetComponent<SewerLevel>();
        levelScript.AddLunaKey();
    }

    public void IsClosed()
    {
        animator.SetBool("isOpenChest", false);
    }
}

