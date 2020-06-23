using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberButtonsManager : MonoBehaviour
{
    public GameObject numberPrefab;
    int numfloors=20;
    public int lineCount=8;
    public float interval=0.2f;
    public float height=10;
    public float width=0;
    SingleElevator elevatorScript;
    List<NumberButton> buttonList=new List<NumberButton>();
    // Start is called before the first frame update
    void Start()
    {   
        Common.Common common=GameObject.Find("Objects").GetComponent<Common.Common>();
        numfloors=common.floorCount;
        
        Transform elevator=transform.parent.parent;
        elevatorScript=elevator.GetComponent<SingleElevator>();
        for(int i=1;i<=numfloors;i++)                               //创建楼层按钮
        {
            GameObject numButton=Instantiate(numberPrefab);
            numButton.transform.SetParent(transform);
            numButton.GetComponent<NumberButton>().setElevator(elevatorScript);
            float x=(i-1)/lineCount*interval;
            float y=height-(i-1)%lineCount*interval;
            // numButton.transform.position=transform.TransformVector(new Vector3(x,y,0));
            numButton.transform.position=transform.TransformPoint(new Vector3(x,y,0));
            NumberButton number=numButton.GetComponent<NumberButton>();
            number.Number=i;
            buttonList.Add(number);
        }
    }

    public void setButtonState(int floor,bool state)        //设置楼层按钮状态
    {
        Debug.Log(floor);
        NumberButton button=buttonList[floor-1];
        button.setEnabled(state);
    }
}
