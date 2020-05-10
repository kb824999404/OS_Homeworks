using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using TMPro;
public class MultiElevator : MonoBehaviour
{
    public GameObject elevatorPrefab;
    public GameObject globalButtonPrefab;
    int elevatorCount=4;
    int floorCount=20;
    float floorHeight=8;
    public float distance=10;
    List<SingleElevator> elevatorList=new List<SingleElevator>();
    List<List<GameObject>> globalButtonList=new List<List<GameObject>>();
    // Start is called before the first frame update
    void Start()
    {
        Common.Common common=GameObject.Find("Objects").GetComponent<Common.Common>();
        floorCount=common.floorCount;
        elevatorCount=common.elevatorCount;
        floorHeight=common.floorHeight;
        for(int i=0;i<elevatorCount;i++)                        //创建电梯
        {
            GameObject elevator=Instantiate(elevatorPrefab);
            elevator.transform.SetParent(transform);
            float x=distance*i;
            Vector3 pos=new Vector3(x,0,0);
            elevator.transform.position=transform.TransformPoint(pos);
            elevator.GetComponent<SingleElevator>().ID=i;
            elevatorList.Add(elevator.GetComponent<SingleElevator>());
        }
        Transform globalButtons=GameObject.Find("GlobalButtons").transform;
        for(int i=0;i<floorCount;i++)                               //创建上下行按钮
        {
            List<GameObject> buttonList=new List<GameObject>();
            for(int j=0;j<elevatorCount;j++)
            {
                GameObject globalButton=Instantiate(globalButtonPrefab);
                globalButton.transform.SetParent(globalButtons);
                float y=floorHeight*i;
                float x=distance*j;
                Vector3 pos=new Vector3(x,y,0);
                globalButton.transform.position=globalButtons.TransformPoint(pos);
                globalButton.transform.GetChild(0).GetComponent<GlobalButton>().floor=i+1;
                globalButton.transform.GetChild(1).GetComponent<GlobalButton>().floor=i+1;
                globalButton.transform.GetChild(0).GetComponent<GlobalButton>().ID=j;
                globalButton.transform.GetChild(1).GetComponent<GlobalButton>().ID=j;
                buttonList.Add(globalButton);
            }
            globalButtonList.Add(buttonList);
        }
    }
    public void dispatch(int eleID,int floor,UserCommand command)
    {
        if(elevatorList[eleID].enabled)                        //如果该电梯在当前楼层，且处于静止状态，则直接开门
        {
            SingleElevator elevator=elevatorList[eleID];
            if(elevator.elevetorState==ElevetorState.STANDSTILL&&elevator.currentFloor==floor)
            {
                elevator.doorControl(UserCommand.OPEN);
                return;
            }
        }
        List<SingleElevator> EnabledList=new List<SingleElevator>();
        foreach(SingleElevator elevator in elevatorList)                //筛选可用电梯
        {
            if(elevator.eleEnabled)
            {
                EnabledList.Add(elevator);
            }
        }
        int bestIndex=0;
        List<int> dist=new List<int>();                 //可使用电梯距离用户的距离
        for(int i=0;i<EnabledList.Count;i++)
        {
            if(EnabledList[i].elevetorState==ElevetorState.RUNNING_UP&&
            command==UserCommand.GOUP&&
            floor>EnabledList[i].currentFloor)                          //向上顺路
            {
                dist.Add(floor-EnabledList[i].currentFloor);
            }
            else if(EnabledList[i].elevetorState==ElevetorState.RUNNING_DOWN&&
            command==UserCommand.GODOWN&&
            floor<EnabledList[i].currentFloor)                          //向下顺路
            {
                dist.Add(EnabledList[i].currentFloor-floor);
            }
            else if(EnabledList[i].elevetorState==ElevetorState.STANDSTILL)//电梯静止
            {
                dist.Add(Mathf.Abs(EnabledList[i].currentFloor-floor));
            }
            else
            {
                dist.Add(100);
            }
            if(i!=bestIndex)                            
            {
                if(dist[i]<dist[bestIndex])                     //寻找最短距离电梯
                {
                    bestIndex=i;
                }
            }
        }
        if(dist[bestIndex]==0)                  //最佳电梯在当前楼层
        {
            EnabledList[bestIndex].doorControl(UserCommand.OPEN);        //打开门等待用户关闭
        }
        else
        {
            EnabledList[bestIndex].eleMove(floor);                      //加入最佳电梯消息队列
            if(command==UserCommand.GOUP)                               //禁用该楼层所有上/下行按钮
            {
                //禁用上行按钮
                foreach(GameObject globalButton in globalButtonList[floor-1])
                {
                    globalButton.transform.GetChild(0).GetComponent<GlobalButton>().setEnabled(false);
                }
            }
            else
            {
                //禁用下行按钮
                foreach(GameObject globalButton in globalButtonList[floor-1])
                {
                    globalButton.transform.GetChild(1).GetComponent<GlobalButton>().setEnabled(false);
                }

            }
        }

    }
    public void enableButton(int floor)
    {
        foreach(GameObject globalButton in globalButtonList[floor-1])
        {
            globalButton.transform.GetChild(0).GetComponent<GlobalButton>().setEnabled(true);
            globalButton.transform.GetChild(1).GetComponent<GlobalButton>().setEnabled(true);
        }
    }
}
