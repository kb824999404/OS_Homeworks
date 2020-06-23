using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common
{
    public enum ElevetorState      //电梯状态
    {
        STANDSTILL,RUNNING_UP,RUNNING_DOWN,     //静止，上行，下行
    };
    public enum DoorState
    {
        OPEN,CLOSED,                 //门开，门关
        NOPE,READYSTART,READYSTOP               //空动画，即将启动，即将停止

    }
    public enum UserCommand       //用户操作
    {
        GOUP,GODOWN,                 //上行，下行
        OPEN,CLOSE                  //开门，关门
    };
    public class Common:MonoBehaviour
    {
        public float floorHeight=6;
        public int floorCount=20;
        public int elevatorCount=4;
        void Awake()
        {
            floorCount=PlayerPrefs.GetInt("floorCount");
            elevatorCount=PlayerPrefs.GetInt("elevatorCount");
        }
        public void ChangeScene()
        {
            SceneManager.LoadScene("StartScene");
        }
    }
}
