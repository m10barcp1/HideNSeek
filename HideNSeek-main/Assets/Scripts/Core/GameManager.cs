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

    [Header("Setting")]
    public GameObject Player;
    public GameObject[] Character;
    //private List<Vector3> InitTransform;
    [HideInInspector]
    public bool onClick = false;
    [HideInInspector]
    public bool StartGame, WinGame, LoseGame, EndGame;

    [Header("Panel To Show")]
    public GameObject WinPanel;
    public GameObject LosePanel;

    private float TimePlay, TimeStartUp;
    [Header("Text")]
    public Text CountDownTime;
    public Text CountDownToStartUp;
    public GameObject joystick;


    private int indexCurentLevel;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        indexCurentLevel = 1;
    }
    private void Start()
    {
        joystick.SetActive(false);
    }

    private void Update()
    {
        if (onClick)
        {
            var SeekPlayer = Player.transform.GetChild(0).GetComponent<SeekStateManager>();
            var HidePlayer = Player.transform.GetChild(1).GetComponent<HideStateManager>();
            CountDownToStartUp.enabled = true;
            TimeStartUp -= Time.deltaTime;
            CountDownToStartUp.text = Mathf.Round(TimeStartUp).ToString();
            
            if(TimeStartUp <= 0)
            {
                if (StateOfGame == GameState.seek)
                {
                    TurnOffAllModelOfHideCharacter();
                }
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
                    }else if(StateOfGame == GameState.seek)
                    {
                        Debug.Log($"CharacerInImprison: {SeekPlayer.CharacerInImprison} and Character.Length: {Character.Length}");
                        if(SeekPlayer.CharacerInImprison > Character.Length/2)
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
        else
        {
            TimePlay = 10f;
            TimeStartUp = 4f;
            StartGame = false;
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
        if (!onClick)
        {
            StateOfGame = GameState.seek;
            onClick = true;
            joystick.SetActive(true);
            InsSeek();
        }
    }
    public void OnHideState()
    {
        if (!onClick)
        {
            StateOfGame = GameState.hide;
            onClick = true;
            joystick.SetActive(true);
            InsHide();
        }
    }
    public void InsHide()
    {
        Character[0].transform.GetChild(0).GetComponent<SeekStateManager>().CharacerInImprison = 0;
        Character[0].transform.GetChild(0).gameObject.SetActive(true);
        Character[0].transform.GetChild(1).gameObject.SetActive(false);

    }
    public void InsSeek() 
    {
        Player.transform.GetChild(0).gameObject.SetActive(true);
        Player.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void BeforeStartGame()
    {
        // Set state
        StartGame = false; EndGame = false;
        WinGame = false;  LoseGame = false;         
        onClick = false;
        

        //SeekPlayer
        Player.transform.GetChild(0).gameObject.SetActive(false);
        Player.transform.GetChild(0).transform.position = Vector3.zero;
        //HidePlayer
        Player.transform.GetChild(1).gameObject.SetActive(true);
        Player.transform.GetChild(1).GetComponent<HideStateManager>().ResetState();

        for (int j = 0; j < Character.Length; j++)
        {
            //Seek
            Character[j].transform.GetChild(0).gameObject.SetActive(false);
            Character[j].transform.GetChild(0).transform.position = Vector3.zero;

            //Hide
            Character[j].transform.GetChild(1).gameObject.SetActive(true);
            Character[j].transform.GetChild(1).GetComponent<HideStateManager>().ResetState();
        }
    }
    public void TurnOffAllModelOfHideCharacter()
    {
        foreach(GameObject i in Character)
        {
            i.transform.GetChild(1).GetComponent<HideStateManager>().TurnOffModel();
        }
    }
    public void WinGameAction()
    {
        Player.transform.GetChild(1).GetComponent<MovementPlayer>().MovementState(0);
        Player.transform.GetChild(1).GetComponent<MovementPlayer>().SetStateIdle();
        WinPanel.SetActive(true);
        joystick.SetActive(false);
    }
    public void LoseGameAction()
    {
        Player.transform.GetChild(1).GetComponent<MovementPlayer>().MovementState(0);
        Player.transform.GetChild(1).GetComponent<MovementPlayer>().SetStateIdle();
        LosePanel.SetActive(true);
        joystick.SetActive(false);
    }

    public void OnClickPlayAgain()
    {
        //onClick = false;
        //LoseGame = false;

        WinPanel.SetActive(false);
        BeforeStartGame();
        LosePanel.SetActive(false);
       

        
    }
    public void OnClickNextLevel()
    {
        WinPanel.SetActive(false);
        BeforeStartGame();

        indexCurentLevel++;
        InsLevel(indexCurentLevel);
        
    }
    private void InsLevel(int index)
    {
        GameObject CurrentLevelOfGame = GameObject.Find("Level" + (index - 1).ToString());
        Destroy(CurrentLevelOfGame);
        string nameCurrentLevel = "Level" + index.ToString();
        GameObject currentLoadLevel = Resources.Load<GameObject>($"Level/{nameCurrentLevel}");
        GameObject currentLevel = Instantiate(currentLoadLevel);



    }
}
