using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartScene : MonoBehaviour
{
    int elevatorCount=4;
    int floorCount=20;
    public GameObject ele;
    public GameObject floor;
    TextMeshProUGUI eleLabel;
    TextMeshProUGUI floorLabel;

    // Start is called before the first frame update
    void Start()
    {
        eleLabel=ele.GetComponent<TextMeshProUGUI>();
        floorLabel=floor.GetComponent<TextMeshProUGUI>();
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void ChangeScene()
    {
        PlayerPrefs.SetInt("floorCount",floorCount);
        PlayerPrefs.SetInt("elevatorCount",elevatorCount);
        SceneManager.LoadScene("MainScene");
    }
    public void eleAdd()
    {
        if(elevatorCount>=5)    return;
        elevatorCount++;
        eleLabel.text=elevatorCount.ToString();
    }
    public void eleMinus()
    {
        if(elevatorCount<=1)    return;
        elevatorCount--;
        eleLabel.text=elevatorCount.ToString();
    }
    public void floorAdd()
    {
        if(floorCount>=30)  return;
        floorCount++;
        floorLabel.text=floorCount.ToString();
    }
    public void floorMinus()
    {
        if(floorCount<=1)   return;
        floorCount--;
        floorLabel.text=floorCount.ToString();
    }

}
