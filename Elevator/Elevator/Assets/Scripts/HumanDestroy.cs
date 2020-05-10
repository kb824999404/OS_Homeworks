using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class HumanDestroy : MonoBehaviour
{
    Animator animator;
    AnimatorStateInfo animatorStateInfo;
    public SingleElevator singleElevator;
    public bool isIn=true;
    // Start is called before the first frame update
    void Start()
    {
        animator=GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animatorStateInfo=animator.GetCurrentAnimatorStateInfo(0);
        if(animatorStateInfo.normalizedTime>=1.0f&&animatorStateInfo.IsName("turnBack"))
        {
            GameObject.Destroy(gameObject);
            if(isIn)
            {
                singleElevator.doorState=DoorState.READYSTART;
            }
            else
            {
                singleElevator.doorState=DoorState.READYSTOP;
            }
        }
    }
}
