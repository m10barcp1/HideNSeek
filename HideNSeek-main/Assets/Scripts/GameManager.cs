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
    public List<Transform> InitTransform;
    public bool onClick = false;
    public bool StartGame, WinGame, LoseGame, EndGame = false;
    public GameObject WinPanel, LosePanel;

    private float TimePlay, TimeStartUp;

    public Text CountDownTime, CountDownToStartUp;

    public GameObject joystick;

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
        
    }
    private void Start()
    {
        joystick.SetActive(false);
        foreach (GameObject i in Character)
        {
            InitTransform.Add(i.transform);
        }
    }

    private void Update()
    {
        if (onClick)
        {
            var SeekPlayer = Player.GetComponent<SeekStateManager>();
            var HidePlayer = Player.GetComponent<HideStateManager>();
            TimeStartUp -= Time.deltaTime;
            CountDownToStartUp.text = Mathf.Round(TimeStartUp).ToString();
            if(TimeStartUp <= 0)
            {
                StartGame = true;
                UpdateTimePlay();
                // X? lý trong ván
                if (HidePlayer != null)
                {
                    if (HidePlayer.IsImprisoned)
                    {
                        LoseGame = true;
                        LoseGameAction();
                    }
                }
                


                // X? lý khi k?t thúc ván
                if (TimePlay <= 0)
                {
                    if(StateOfGame == GameState.hide){
                        if (!LoseGame)
                        { 
                            WinGameAction();
                        }
                    }else
                    {
                        if(Player.GetComponent<SeekStateManager>().CharacerInImprison > Character.Length/2)
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
    void UpdateTimePlay()
    {
        CountDownToStartUp.enabled = false;
        TimePlay = Mathf.Max(TimePlay - Time.deltaTime, 0);
        CountDownTime.text = Mathf.Round(TimePlay).ToString();
    }
    public void OnSeekState()
    {
        //onClick = true;
        //StateOfGame = GameState.seek;
        
        //InsSeek();
        //WinPanel.SetActive(true);
        
    }
    public void OnHideState()
    {
        StateOfGame = GameState.hide;
        onClick = true;
        joystick.SetActive(true);
        InsHide(2);
    }
    public void InsHide(int i)
    {
        Character[0].transform.GetChild(0).gameObject.SetActive(true);
        Character[0].transform.GetChild(1).gameObject.SetActive(false);
        Player.transform.GetChild(0).gameObject.SetActive(false);
        Player.transform.GetChild(1).gameObject.SetActive(true);

    }
    public void InsSeek() 
    {
        Player.transform.GetChild(0).gameObject.SetActive(true);
        Player.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void BeforeStartGame()
    {
        
        StartGame = false; EndGame = false;
        WinGame = false;  LoseGame = false;
        onClick = false;
        CharacterInImprison = 0;
        for (int j = 1; j < Character.Length; j++)
        {
            Character[j].gameObject.transform.position = InitTransform[j].position;
            Character[j].SetActive(true);
        }
    }
    public void WinGameAction()
    {
        WinPanel.SetActive(true);
        joystick.SetActive(false);
    }
    public void LoseGameAction()
    {
        LosePanel.SetActive(true);
        joystick.SetActive(false);
    }

    public void OnClickPlayAgain()
    {
        //onClick = false;
        //LoseGame = false;

        WinPanel.SetActive(false);
        LosePanel.SetActive(false);
        BeforeStartGame();

        
    }
    public void OnClickNextLevel()
    {

    }

}
