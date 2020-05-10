using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorView : MonoBehaviour
{
    ChangeView changeView;
    Material material;
    Color originColor;
    bool hoverEnabled=true;
    public void setEnabled(bool state)
    {
        hoverEnabled=state;
    }
    void Start()
    {
        material=GetComponent<MeshRenderer>().material;
        originColor=material.color;
        changeView=GameObject.Find("Elevators").GetComponent<ChangeView>();
    }
    void OnMouseEnter()
    {
        material.color=Color.red;

    }
    void OnMouseExit()
    {
        material.color=originColor;
    }
    void OnMouseDown()
    {
        changeView.setFloorView(transform.position.y);
    }
}
