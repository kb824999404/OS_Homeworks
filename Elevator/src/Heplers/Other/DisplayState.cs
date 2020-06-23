using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using TMPro;

public class DisplayState : MonoBehaviour
{
    int floor=1;
    int ID;
    ElevetorState state=ElevetorState.STANDSTILL;
    TextMeshPro flootText;
    TextMeshPro stateText;
    FloorManager floorManager;
    // Start is called before the first frame update
    void Start()
    {
        var texts=GetComponentsInChildren<TextMeshPro>();
        flootText=texts[0];
        stateText=texts[1];
        ID=transform.parent.gameObject.GetComponent<SingleElevator>().ID;
        floorManager=GameObject.Find("Floors").GetComponent<FloorManager>();
    }
    public void setFloor(int num)
    {
        floor=num;
        flootText.text=floor.ToString();
        floorManager.changeState(ID,floor,state);
    }
    public void setState(ElevetorState s)
    {
        state=s;
        if(state==ElevetorState.STANDSTILL)
        {
            stateText.text="";
        }
        else if(state==ElevetorState.RUNNING_UP)
        {
            stateText.text="↑";
        }
        else if(state==ElevetorState.RUNNING_DOWN)
        {
            stateText.text="↓";
        }
        floorManager.changeState(ID,floor,s);
    }
    public void setDisabled()
    {
        flootText.text="X";
        flootText.color=Color.red;
        stateText.text="";
        floorManager.setDisabled(ID);
    }

}
