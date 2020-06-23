using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    Material material;
    Color originColor;
    bool hoverEnabled=true;
    public void setEnabled(bool state)
    {
        hoverEnabled=state;
    }
    void Start()
    {
        Material material=GetComponent<MeshRenderer>().materials[0];
        originColor=material.color;
    }
    void OnMouseEnter()
    {
        if(hoverEnabled)
        {
            Transform elevator=transform.parent;
            for(int i=0;i<6;i++)
            {
                Material material=elevator.GetChild(i).GetComponent<MeshRenderer>().material;
                material.color=Color.red;
            }
        }

    }
    void OnMouseExit()
    {
        Transform elevator=transform.parent;
        for(int i=0;i<6;i++)
        {
            Material material=elevator.GetChild(i).GetComponent<MeshRenderer>().material;
            material.color=originColor;
        }
    }
}
