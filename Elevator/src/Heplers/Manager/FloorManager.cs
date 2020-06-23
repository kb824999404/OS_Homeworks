using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using TMPro;

public class FloorManager : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject floorNumPrefab;
    public GameObject framePrefab;
    public GameObject eleStatePrefab;
    public GameObject wall;
    public GameObject lwall;
    public GameObject rwall;
    int floorCount=20;
    int elevatorCount=4;
    float floorHeight=8;
    public int distance=15;
    public Vector2 offset=new Vector2(-5,5);
    public Vector2 offset1=new Vector2(0,0);
    List< List<GameObject> >stateList=new  List<List<GameObject>>();
    // Start is called before the first frame update
    void Start()
    {
        Common.Common common=GameObject.Find("Objects").GetComponent<Common.Common>();
        floorCount=common.floorCount;
        elevatorCount=common.elevatorCount;
        floorHeight=common.floorHeight;
        for(int i=0;i<=floorCount;i++)                              //创建楼层
        {
            GameObject floor=Instantiate(floorPrefab);
            floor.transform.SetParent(transform);
            float y=floorHeight*i;
            Vector3 pos=new Vector3(2,y,0);
            floor.transform.position=transform.TransformPoint(pos);
        }
        Transform floorNums=GameObject.Find("FloorNums").transform;
        for(int i=0;i<floorCount;i++)                           //创建楼层数字
        {
            GameObject floorNum=Instantiate(floorNumPrefab);
            floorNum.transform.SetParent(floorNums);
            float y=floorHeight*i;
            Vector3 pos=new Vector3(0,y,0);
            floorNum.transform.position=floorNums.TransformPoint(pos);
            floorNum.GetComponent<TextMeshPro>().text=(i+1).ToString();
        }
        for(int i=0;i<elevatorCount;i++)                            //创建楼层电梯门口的电梯状态
        {
            List<GameObject>tempList=new List<GameObject>();
            for(int j=0;j<floorCount;j++)
            {
                GameObject eleState=Instantiate(eleStatePrefab);
                eleState.transform.SetParent(transform);
                float x=distance*i+offset.x;
                float y=floorHeight*j+offset.y;
                Vector3 pos=new Vector3(x,y,2);
                eleState.transform.position=transform.TransformPoint(pos);
                tempList.Add(eleState);
            }
            stateList.Add(tempList);
        }
        for(int i=0;i<5;i++)                                        //创建门框
        {
            for(int j=0;j<floorCount;j++)
            {
                GameObject frame=Instantiate(framePrefab);
                frame.transform.SetParent(transform);
                float x=distance*i+offset1.x;
                float y=floorHeight*j+offset1.y;
                Vector3 pos=new Vector3(x,y,2.75f);
                frame.transform.position=transform.TransformPoint(pos);
            }
        }
        {                                                           //根据楼层数调整背景墙大小
            Vector3 pos=wall.transform.position;
            pos.y=floorHeight*floorCount/2-0.5f;
            pos.z=3;
            wall.transform.position=transform.TransformPoint(pos);
            Vector3 scale=wall.transform.localScale;
            scale.y=floorHeight*floorCount;
            wall.transform.localScale=scale;
        }
        {
            Vector3 pos=lwall.transform.position;                   //根据楼层数调整左侧墙大小
            pos.y=floorHeight*floorCount/2+0.5f;
            pos.z=0;
            lwall.transform.position=transform.TransformPoint(pos);
            Vector3 scale=lwall.transform.localScale;
            scale.y=floorHeight*floorCount+1;
            lwall.transform.localScale=scale;
        }
        {
            Vector3 pos=rwall.transform.position;                    //根据楼层数调整右侧墙大小
            pos.y=floorHeight*floorCount/2+0.5f;
            pos.z=0;
            rwall.transform.position=transform.TransformPoint(pos);
            Vector3 scale=rwall.transform.localScale;
            scale.y=floorHeight*floorCount+1;
            rwall.transform.localScale=scale;
        }

    }
    public void changeState(int ID,int floor,ElevetorState state)
    {
        foreach(GameObject g in stateList[ID])
        {
            TextMeshPro[] texts=g.GetComponentsInChildren<TextMeshPro>();
            texts[0].text=floor.ToString();
            if(state==ElevetorState.STANDSTILL)
            {
                texts[1].text="";
            }
            else if(state==ElevetorState.RUNNING_UP)
            {
                texts[1].text="↑";
            }
            else if(state==ElevetorState.RUNNING_DOWN)
            {
                texts[1].text="↓";
            }
        }
    }
    public void setDisabled(int ID)
    {
        foreach(GameObject g in stateList[ID])
        {
            TextMeshPro[] texts=g.GetComponentsInChildren<TextMeshPro>();
            texts[0].text="X";
            texts[1].text="";
        }
    }
}
