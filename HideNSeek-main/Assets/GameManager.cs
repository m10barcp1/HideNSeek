using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum GameState
    {
        hide,
        seek
    }
    public GameState StateOfGame;

    public GameObject Player;
    public GameObject[] Character;
    public bool onClick = false;
    public bool StartGame, WinGame, LoseGame, EndGame = false;
    private float TimePlay, TimeStartUp;

    public Text CountDownTime, CountDownToStartUp;

    private int CharacterInImprison;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        TimePlay = 10f;
        TimeStartUp = 4f;
        StartGame = false;
        CharacterInImprison = 0;
    }

    private void Update()
    {
        if (onClick)
        {
            TimeStartUp -= Time.deltaTime;
            CountDownToStartUp.text = Mathf.Round(TimeStartUp).ToString();
            if(TimeStartUp <= 0)
            {
                UpdateTimePlay();
                

                if (TimePlay <= 0)
                {
                    if(StateOfGame == GameState.hide){
                        if (!LoseGame)
                        { 
                            WinGameAction();
                        }
                    }else
                    {
                        if(Player.GetComponent<SeekStateManager>().CharacerInImprison > Character.Length)
                        {
                            WinGameAction();
                        }
                        else
                        {
                            LoseGameAction();
                        }
                    }
                }
            }
        }
    }

    public void OnClickPause()
    {
        Time.timeScale = 0;
    }
    void UpdateTimePlay()
    {
        CountDownToStartUp.enabled = false;
        TimePlay = Mathf.Max(TimePlay - Time.deltaTime, 0);
        CountDownTime.text = Mathf.Round(TimePlay).ToString();
    }
    public void OnSeekState()
    {
        onClick = true;
        StateOfGame = GameState.seek;
        
        InsSeek();
    }
    public void OnHideState()
    {
        StateOfGame = GameState.hide;
        onClick = true;
        InsHide(2);
    }
    public void InsHide(int i)
    {
        //Transform pointToIns = Hide.transform.GetChild(2).transform;
        //Character.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        //Character.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
    }
    public void InsSeek() 
    {
        Player.transform.GetChild(0).gameObject.SetActive(true);
        Player.transform.GetChild(1).gameObject.SetActive(false);
    }
    public void WinGameAction()
    {
        // Voi SeekMode
        // Khi tim duoc so HideCharacter > 1/2 tong so ban dau va het gio
        if (StateOfGame == GameState.seek)
        {
            if (CharacterInImprison > Character.Length / 2)
            {
                WinGame = true;

            }
        }
        // Voi HideMode
        else
        {
            // Khi khong bi bat cho toi het gio
            if (Player.GetComponent<HideStateManager>().IsImprisoned)
            {

            }


            // Co the theo Misssion trong tuong lai
        }
    }

    public void LoseGameAction()
    {

        // Show Panel LoseGame
        TimePlay = 0;
        LoseGame = true;


        




        //Voi SeekMode
        // Khi tim duoc so HideCharacter < 1/2 tong so ban dau va het gio
            
        // Voi HideMode

        // Bi bat khi chua het gio


        // Khong hoan thanh mission


    }

}
