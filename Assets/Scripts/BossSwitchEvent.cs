using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSwitchEvent : EventController {

    [SerializeField] GameObject BossSwitch;

    [SerializeField] Vector3 bossSwitchPosition;

    protected override void Start()
    {
        Debug.Log("Boss switch event start");
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }


    protected override IEnumerator EventRoutine()
    {
        Instantiate(BossSwitch, bossSwitchPosition, Quaternion.identity);
        Debug.Log("Boss switch instantiation");
        yield return true;
        
        //return base.EventRoutine();
    }
}
