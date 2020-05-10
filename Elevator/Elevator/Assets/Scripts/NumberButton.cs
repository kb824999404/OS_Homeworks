using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberButton : MonoBehaviour
{
    int number=1;
    TextMeshPro text;
    SingleElevator elevatorScript;
    bool buttonEnabled=true;
    void Awake()
    {
        text=GetComponentInChildren<TextMeshPro>();
    }
    public void setElevator(SingleElevator elevator)
    {   
        elevatorScript=elevator;
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
    public int Number{
        get{
            return number;
        }
        set{
            number=value;
            text.text=number.ToString();
        }
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
            elevatorScript.eleMove(Number);
        }

    }
}
