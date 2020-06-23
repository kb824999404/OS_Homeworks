using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ChangeView : MonoBehaviour
{
    GameObject elevatorCamera;
    public GameObject floorCamera;
    public GameObject UI1;
    public GameObject UI2;
    public float cameraSpeed=1;
    Material material;
    Color originColor;
    bool innerViewEnabled=false;
    bool floorViewEnabled=false;
    float minHeight=0;
    float maxHeight;
    void Start()
    {
        Common.Common common=GameObject.Find("Objects").GetComponent<Common.Common>();
        maxHeight=(common.floorCount+1)*common.floorHeight;
    }
    void Update()
    {
        if(!innerViewEnabled)
        {
            if(Input.GetMouseButton(0))
            {
                Camera camera=Camera.main;
                if(floorViewEnabled)
                {
                    camera=floorCamera.GetComponent<Camera>();
                }
                Ray ray=camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray,out hit))
                {
                    if(hit.transform.tag=="Elevator")
                    {

                        Transform temp=hit.transform;
                        if(temp.parent.tag!="Elevator")
                        {
                            temp=temp.parent.parent;
                        }
                        else
                        {
                            temp=temp.parent;
                        }
                        elevatorCamera=temp.Find("Camera").gameObject;
                        elevatorCamera.SetActive(true);
                        innerViewEnabled=true;
                        UI1.SetActive(false);
                        UI2.SetActive(true);
                        if(floorViewEnabled)
                        {
                            floorCamera.SetActive(false);
                            floorViewEnabled=false;
                        }
                        ChangeColor[] changeColors=GetComponentsInChildren<ChangeColor>();
                        foreach(ChangeColor c in changeColors)
                        {
                            c.setEnabled(false);
                        }
                    }
                }
            }
        }
        if(floorViewEnabled)
        {
            innerViewEnabled=false;
            Vector3 pos=floorCamera.transform.position;
            pos.y+=Input.GetAxis("Mouse ScrollWheel")*cameraSpeed;
            if(pos.y<=minHeight)    pos.y=minHeight;
            else if(pos.y>=maxHeight)   pos.y=maxHeight;
            floorCamera.transform.position=pos;

        }
        if(Input.GetMouseButton(1))
        {
            returnMainView();
        }

    }
    public void returnMainView()
    {
        floorCamera.SetActive(false);
        floorViewEnabled=false;
        UI1.SetActive(true);
        UI2.SetActive(false);
        if(elevatorCamera)
        {
            elevatorCamera.SetActive(false);
            innerViewEnabled=false;
            ChangeColor[] changeColors=GetComponentsInChildren<ChangeColor>();
            foreach(ChangeColor c in changeColors)
            {
                c.setEnabled(true);
            }
        }        
    }
    public void setFloorView(float height)
    {
        
        floorCamera.SetActive(true);
        floorViewEnabled=true;
        UI1.SetActive(false);
        UI2.SetActive(true);
        Vector3 pos=floorCamera.transform.position;
        pos.y=height;
        floorCamera.transform.position=pos;

        if(innerViewEnabled)
        {
            if(elevatorCamera)
            {
                elevatorCamera.SetActive(false);
                innerViewEnabled=false;
                ChangeColor[] changeColors=GetComponentsInChildren<ChangeColor>();
                foreach(ChangeColor c in changeColors)
                {
                    c.setEnabled(true);
                }
            }
        }

    }
}
