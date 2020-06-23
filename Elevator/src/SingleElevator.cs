using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class SingleElevator : MonoBehaviour
{
    public int ID;      //电梯编号
    float floorHeight=6;
    int floorCount=20;
    public bool eleEnabled=true;     //电梯是否可用
    bool isArrived=true;      //是否到达下一楼层
    public int currentFloor=1;   //当前楼层
    int nextFloor=1;      //下一目标楼层
    float liftSpeed=0.5f;         //电梯移动速度
    public ElevetorState elevetorState=ElevetorState.STANDSTILL;
    public DoorState doorState=DoorState.CLOSED;
    List<int> messageQueue=new List<int>(),     //内部消息队列，顺路
    messageQueue_reverse=new List<int>();      //不顺路消息队列

    Animator doorAnimator;                      //电梯门动画控制类
    DisplayState displayState;                  //电梯楼层和状态显示类
    NumberButtonsManager numberButtonsManager;          //楼层按钮管理类
    MultiElevator multiElevator;                    //多电梯管理类
    public OtherButton warnButton,openButton,closeButton;
    public GameObject humanPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Common.Common common=GameObject.Find("Objects").GetComponent<Common.Common>();
        floorHeight=common.floorHeight;
        floorCount=common.floorCount;

        doorAnimator=GetComponentInChildren<Animator>();
        displayState=GetComponentInChildren<DisplayState>();
        numberButtonsManager=GetComponentInChildren<NumberButtonsManager>();
        multiElevator=GetComponentInParent<MultiElevator>();
        InvokeRepeating("updateEleState",1,1);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos=transform.position;
        if(elevetorState!=ElevetorState.STANDSTILL)     //电梯处于运行状态
        {
            if(!isArrived)          //还没到达目标楼层
            {
                float y=pos.y,nexty=(nextFloor-1)*floorHeight;          //下一楼层高度
                if(nextFloor>currentFloor)          //向上运动
                {
                    y=pos.y+liftSpeed;
                    if(y>=nexty)                //到达
                    {
                        y=nexty;
                        isArrived=true;
                        currentFloor=nextFloor;
                    }
                }
                else if(nextFloor<currentFloor)     //向下运动
                {
                    y=pos.y-liftSpeed;
                    if(y<=nexty)                //到达
                    {
                        y=nexty;
                        isArrived=true;
                        currentFloor=nextFloor;
                    }
                }
                transform.position=new Vector3(pos.x,y,pos.z);
            }
        }
    }
    public void doorControl(UserCommand command)                 //开关门
    {
        if(command==UserCommand.OPEN)           //用户要开门
        {
            if((doorState==DoorState.CLOSED||doorState==DoorState.NOPE)
            &&elevetorState==ElevetorState.STANDSTILL)    //如果门是关闭状态且电梯静止
            {
                doorState=DoorState.OPEN;               //门状态更新为打开
                eleEnabled=false;                   //电梯不可用
                doorAnimator.SetTrigger("Open");        //开门动画
            }
        }
        else if(command==UserCommand.CLOSE)         //用户要关门
        {
            if((doorState==DoorState.OPEN||doorState==DoorState.NOPE)
            &&elevetorState==ElevetorState.STANDSTILL)      //如果门是关闭状态且电梯静止
            {
                doorState=DoorState.CLOSED;         //门状态更新为关闭
                eleEnabled=true;                    //电梯可用
                doorAnimator.SetTrigger("Close");        //关门动画
                multiElevator.enableButton(currentFloor);                   //启用当前楼层电梯外按钮
            }
        }
    }
    public void humanAnim(bool isIn)            //人物动画
    {
        if(isIn)
        {
            GameObject human=Instantiate(humanPrefab);
            human.transform.SetParent(transform);
            human.transform.position=transform.TransformPoint(0,0.6f,-4);
            human.GetComponent<HumanDestroy>().singleElevator=this;
        }
        else
        {
            GameObject human=Instantiate(humanPrefab);
            human.transform.SetParent(transform);
            human.transform.position=transform.TransformPoint(1,0.6f,-2);
            human.transform.rotation=new Quaternion(0,0,0,0);
            human.GetComponent<HumanDestroy>().singleElevator=this;
            human.GetComponent<HumanDestroy>().isIn=false;
        }
    }
    public void warnControl()           //警报键
    {
        eleEnabled=false;                                       //禁用电梯
        for(int i=1;i<=floorCount;i++)
        {
            numberButtonsManager.setButtonState(i,false);        //禁用楼层按钮
        }
        doorAnimator.SetTrigger("Close");                   //关门动画
        doorState=DoorState.NOPE;                       //动画变为空状态
        elevetorState=ElevetorState.STANDSTILL;         //电梯变为静止状态
        displayState.setDisabled();                     //禁用电梯状态显示
        warnButton.setEnabled(false);                   //禁用警报键
        openButton.setEnabled(false);                   //禁用开门键
        closeButton.setEnabled(false);                  //禁用关门键
        StopAllCoroutines();                            //停用定时器
    }
    public void eleMove(int dest)           //命令电梯移动
    {   
        if(currentFloor<dest)           //按键大于当前楼层
        {
            if(elevetorState==ElevetorState.STANDSTILL)     //电梯处于静止状态
            {
                if(!messageQueue.Contains(dest))        //防止重复添加
                {
                    messageQueue.Add(dest);                     //将目标楼层加入消息队列
                }
            }
            else if(elevetorState==ElevetorState.RUNNING_UP)      //电梯正在向上运行
            {
                if(!messageQueue.Contains(dest))
                {
                    messageQueue.Add(dest);                 //将目标楼层加入顺路消息队列并排序
                    messageQueue.Sort();
                }

            }
            else if(elevetorState==ElevetorState.RUNNING_DOWN)  //电梯正在向下运行
            {
                if(!messageQueue_reverse.Contains(dest))
                {
                    messageQueue_reverse.Add(dest);         //将目标楼层加入不顺路消息队列并排序
                    messageQueue_reverse.Sort();
                }
 
            }
            numberButtonsManager.setButtonState(dest,false);        //禁用目标楼层按钮
        }
        else if(currentFloor>dest)      //按键小于当前楼层
        {
            if(elevetorState==ElevetorState.STANDSTILL)     //电梯处于静止状态
            {
                if(!messageQueue.Contains(dest))
                {
                    messageQueue.Add(dest);                     //将目标楼层加入消息队列
                }
            }
            else if(elevetorState==ElevetorState.RUNNING_DOWN)      //电梯正在向下运行
            {
                if(!messageQueue.Contains(dest))
                {
                    messageQueue.Add(dest);                 //将目标楼层加入顺路消息队列并反向排序
                    messageQueue.Sort();
                    messageQueue.Reverse();
                }
            }
            else if(elevetorState==ElevetorState.RUNNING_UP)  //电梯正在向上运行
            {
                if(!messageQueue_reverse.Contains(dest))
                {
                    messageQueue_reverse.Add(dest);         //将目标楼层加入不顺路消息队列并反向排序
                    messageQueue_reverse.Sort();
                    messageQueue_reverse.Reverse();
                }
            }
            numberButtonsManager.setButtonState(dest,false);        //禁用目标楼层按钮
        }
        else                        //按键为当前楼层
        {
            if(elevetorState==ElevetorState.STANDSTILL)             //电梯静止，打开门
            {
                doorState=DoorState.OPEN;
                doorAnimator.SetTrigger("Open");       //开门动画
            }

        }
    }
    void updateEleState()                       //处理消息队列，更新电梯状态
    {
        if(!eleEnabled) return;             //电梯已被禁用
        if(messageQueue.Count!=0)
        {
            if(doorState==DoorState.OPEN)   return;         //如果电梯门开着，等待关门
            if(elevetorState==ElevetorState.STANDSTILL)     //电梯静止
            {
                doorState=DoorState.OPEN;               //门状态更新为打开
                doorAnimator.SetTrigger("Open");        //开门动画
                humanAnim(true);
                if(currentFloor<messageQueue[0])            //根据即将运行方向更新电梯状态
                {
                    elevetorState=ElevetorState.RUNNING_UP;
                }
                else if(currentFloor>messageQueue[0])
                {
                    elevetorState=ElevetorState.RUNNING_DOWN;
                }
                // doorState=DoorState.READYSTART;         //就绪启动状态，人物动画播放完后设置


            }
            else if(doorState==DoorState.READYSTART)    //处于就绪启动状态
            {
                doorAnimator.SetTrigger("Close");       //关门动画
                doorState=DoorState.NOPE;               //动画变为空状态
            }
            else if(doorState==DoorState.READYSTOP)     //处于就绪停止状态
            {
                int dest=messageQueue[0];
                messageQueue.RemoveAt(0);               //从消息队列删除目标楼层
                numberButtonsManager.setButtonState(dest,true);        //启用目标楼层按钮
                multiElevator.enableButton(dest);                   //启用目标楼层电梯外按钮
                //TODO:判断之前上行还是下行，只启用一个键
                doorAnimator.SetTrigger("Close");       //关门动画
                doorState=DoorState.NOPE;               //动画变为空状态
                elevetorState=ElevetorState.STANDSTILL;         //电梯变为静止状态
            }
            else                                    //电梯处于移动状态
            {
                if(!isArrived)  return;             //还没到达下一楼层
                int destFloor=messageQueue[0];          //获取第一个目标楼层
                if(currentFloor<destFloor)              //向上运动
                {
                    elevetorState=ElevetorState.RUNNING_UP;     
                    // currentFloor++;                         //当前楼层加一
                    nextFloor=currentFloor+1;               //下一楼层为当前楼层加一
                    isArrived=false;
                }
                else if(currentFloor>destFloor)         //向下运动
                {
                    elevetorState=ElevetorState.RUNNING_DOWN;
                    // currentFloor--;                         //当前楼层减一
                    nextFloor=currentFloor-1;               //下一楼层为当前楼层减一
                    isArrived=false;
                }
                else                                    //到达目标楼层
                {
                    doorState=DoorState.OPEN;
                    doorAnimator.SetTrigger("Open");    //开门动画
                    humanAnim(false);
                    // doorState=DoorState.READYSTOP;      //就绪停止状态，人物动画播放完后设置
                }
            }
            displayState.setFloor(currentFloor);        //在电梯内显示当前楼层
            displayState.setState(elevetorState);       //在电梯内显示当前状态


        }
        else if(messageQueue_reverse.Count!=0)          //如果顺路消息队列为空，不顺路消息队列不为空
        {
            messageQueue=new List<int>(messageQueue_reverse.ToArray());     //交换两个队列
            messageQueue_reverse.Clear();
        }
        
        if(elevetorState==ElevetorState.STANDSTILL)     //电梯在运行过程中禁止点击报警键
        {
            warnButton.setEnabled(true);
        }
        else
        {
            warnButton.setEnabled(false);
        }
    }
}
