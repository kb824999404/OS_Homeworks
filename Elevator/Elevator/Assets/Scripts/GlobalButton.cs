using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using TMPro;

public class GlobalButton : MonoBehaviour
{
    public enum ButtonType{
        UP,DOWN
    };
    public int ID;
    public int floor;
    public ButtonType buttonType;
    TextMeshPro text;
    MultiElevator multiElevator;
    bool buttonEnabled=true;
    void Awake()
    {
        text=GetComponentInChildren<TextMeshPro>();
        multiElevator=GameObject.Find("Elevators").GetComponent<MultiElevator>();
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
        if(buttonEnabled)
        {
            text.color=Color.red;
            if(buttonType==ButtonType.UP)          //上行
            {
                multiElevator.dispatch(ID,floor,UserCommand.GOUP);
            }
            else if(buttonType==ButtonType.DOWN)        //下行
            {
                multiElevator.dispatch(ID,floor,UserCommand.GODOWN);
            }
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
