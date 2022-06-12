using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject Player;
    public GameObject Character;
    
    public bool onClick = false;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    public void OnSeekState()
    {
        onClick = true;
        InsSeek();
    }
    public void OnHideState()
    {
        onClick = true;
        InsHide(2);
    }
    public void InsHide(int i)
    {
        //Transform pointToIns = Hide.transform.GetChild(2).transform;
        Character.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Character.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
    }

    public void InsSeek() 
    {
        Player.transform.GetChild(0).gameObject.SetActive(true);
        Player.transform.GetChild(1).gameObject.SetActive(false);
    }
}
