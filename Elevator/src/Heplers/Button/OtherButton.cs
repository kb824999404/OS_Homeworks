using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using TMPro;

public class OtherButton : MonoBehaviour
{
    public enum ButtonType{
        ALERT,UP,DOWN,OPEN,CLOSE
    };
    public ButtonType buttonType;
    bool buttonEnabled=true;
    TextMeshPro text;
    SingleElevator elevatorScript;
    void Awake()
    {
        text=GetComponentInChildren<TextMeshPro>();
        Transform elevator=transform.parent.parent;
        elevatorScript=elevator.GetComponent<SingleElevator>();
    }
    void OnMouseEnter()
    {
        if(buttonEnabled)
        {
            text.color=Color.yellow;
        }
    }
    void OnMouseExit()
    {
        if(buttonEnabled)
        {
            text.color=Color.white;
        }
    }
    void OnMouseDown()
    {
        if(!buttonEnabled)  return;

        text.color=Color.red;
        if(buttonType==ButtonType.ALERT)            //警报器
        {
            elevatorScript.warnControl();
        }
        else if(buttonType==ButtonType.UP)          //上行
        {

        }
        else if(buttonType==ButtonType.DOWN)        //下行
        {

        }
        else if(buttonType==ButtonType.OPEN)        //开门
        {
            elevatorScript.doorControl(UserCommand.OPEN);
        }
        else if(buttonType==ButtonType.CLOSE)       //关门
        {
            elevatorScript.doorControl(UserCommand.CLOSE);
        }
    }
    public void setEnabled(bool state)
    {
        buttonEnabled=state;
        if(state)
        {
            text.color=Color.white;
        }
        else
        {
            text.color=Color.red;
        }
    }
}
